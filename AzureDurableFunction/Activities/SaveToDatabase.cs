using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using MyFunctionApp.Common;
using MyFunctionApp.Models;
using System.Threading.Tasks;

namespace MyFunctionApp.Activities
{
    public class SaveToDatabase
    {
        readonly IAppConfiguration _appConfig;
        public SaveToDatabase(IAppConfiguration appConfig)
        {
            _appConfig = appConfig;
        }

        [FunctionName(nameof(SaveToDatabase))]
        public async Task Run([ActivityTrigger] RepoModel model, ILogger log)
        {
            log.LogInformation($"Saving item with ID {model?.Id} to Database {_appConfig.SqlConnectionString}...");
            
            // Simulate long running Activity
            await Task.Delay(3000);
        }
    }
}
