using MyFunctionApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFunctionApp.Services
{
    public interface IGitHubClient
    {
        Task<IEnumerable<GitHubApiRepoModel>> GetOrganizationRepos(string orgName);
    }
}