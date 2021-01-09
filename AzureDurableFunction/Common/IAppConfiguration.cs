namespace MyFunctionApp.Common
{
    public interface IAppConfiguration
    {
        string SqlConnectionString { get; }
        string GitHubOrgName { get; }
    }
}
