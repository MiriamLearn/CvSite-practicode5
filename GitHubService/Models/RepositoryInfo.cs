using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubPortfolio.Service.Models
{
    public class RepositoryInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Homepage { get; set; } = string.Empty;
        public int Stars { get; set; }
        public int PullRequests { get; set; }
        public DateTime LastCommitDate { get; set; }
        public Dictionary<string, long> Languages { get; set; } = new Dictionary<string, long>();
    }
}
