using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MyFunctionApp.Activities;
using MyFunctionApp.Common;
using MyFunctionApp.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyFunctionApp
{
    public class Orchestrator
    {
        readonly IAppConfiguration _appConfig;
        public Orchestrator(IAppConfiguration appConfig)
        {
            _appConfig = appConfig;
        }

        [FunctionName(nameof(Orchestrator))]
        public async Task<RepoModel> Run(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger logger)
        {
            if (!context.IsReplaying)
            {
                logger.LogInformation("Starting Orchestrator...");
            }

            var inputData = context.GetInput<InputDataModel>();

            // No input data provided? Fall back to AppSettings,
            // for the local environment it's stored in local.settings.json
            string gitHubOrgName = inputData?.GitHubOrgName ?? _appConfig.GitHubOrgName;

            var repoModel = await context.CallActivityAsync<RepoModel>(nameof(GetGitHubData), gitHubOrgName);
            if (repoModel != null)
            {
                await context.CallActivityAsync(nameof(SaveToDatabase), repoModel);
                
                return repoModel;
            }
            
            return null;
        }

        [FunctionName("Orchestrator_HttpStart")]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            if (req.Method == HttpMethod.Get)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Use HTTP GET for pinging and POST for submitting data")
                };
            }

            // If input is required, validate the Model and return `HttpStatusCode.BadRequest` if fails
            var inputData = await req.Content.ReadAsAsync<InputDataModel>();

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync(nameof(Orchestrator), inputData);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}