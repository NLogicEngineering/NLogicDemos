using System;

namespace MyFunctionApp.Models
{
    public class GitHubApiRepoModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool? @private { get; set; }
        public string html_url { get; set; }
        public string description { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool archived { get; set; }
        public bool disabled { get; set; }
    }
}
