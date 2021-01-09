using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using MyFunctionApp.Models;
using MyFunctionApp.Services;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunctionApp.Activities
{
    public class GetGitHubData
    {
        readonly IGitHubClient _gitHubClient;

        public GetGitHubData(IGitHubClient gitHubClient)
        {
            _gitHubClient = gitHubClient;
        }

        [FunctionName(nameof(GetGitHubData))]
        public async Task<RepoModel> Run([ActivityTrigger] string orgName, ILogger log)
        {
            log.LogInformation($"Getting GitHub {orgName}' repos...");

            var ghModels = await _gitHubClient.GetOrganizationRepos(orgName);
            if (ghModels?.Count() > 0)
            {
                // Find the most recently updated active Repo
                GitHubApiRepoModel ghModel = ghModels
                    .Where(r => !r.disabled)
                    .OrderByDescending(r => r.updated_at ?? r.created_at)
                    .First();

                return new RepoModel
                {
                    Id = ghModel.id,
                    Name = ghModel.name,
                    Description = ghModel.description,
                    IsPrivate = ghModel.@private ?? false,
                    Url = ghModel.html_url,
                    CreatedAt = ghModel.created_at,
                    UpdatedAt = ghModel.updated_at
                };
            }

            return null;
        }

    }
}
