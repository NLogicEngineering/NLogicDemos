using Microsoft.Extensions.Configuration;

namespace MyFunctionApp.Common
{
    public class AppConfiguration : IAppConfiguration
    {
        public string SqlConnectionString { get; private set; }
        public string GitHubOrgName { get; private set; }

        public AppConfiguration(IConfiguration configuration)
        {
            SqlConnectionString = configuration["AppSettings:SqlConnectionString"];
            GitHubOrgName = configuration["AppSettings:GitHubOrgName"];
        }
    }
}
