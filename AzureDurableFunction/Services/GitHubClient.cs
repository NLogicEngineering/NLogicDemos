using MyFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyFunctionApp.Services
{
    public class GitHubClient : IGitHubClient
    {
        readonly HttpClient _httpClient;
        public GitHubClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.github.com/");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "GitHubClient");
        }

        // see https://docs.github.com/en/free-pro-team@latest/rest/reference/repos#list-organization-repositories
        public async Task<IEnumerable<GitHubApiRepoModel>> GetOrganizationRepos(string orgName)
        {
            if (string.IsNullOrEmpty(orgName))
            {
                throw new ArgumentNullException(nameof(orgName));
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"orgs/{orgName}/repos");
            
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                return await JsonSerializer.DeserializeAsync<List<GitHubApiRepoModel>>(responseStream);
            }
        }
    }
}