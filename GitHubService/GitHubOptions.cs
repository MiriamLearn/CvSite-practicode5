using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubPortfolio.Service
{
    public class GitHubOptions
    {
        public const string GitHub = "GitHub";
        public string Username { get; set; } = string.Empty;
        public string PersonalAccessToken { get; set; } = string.Empty;
    }
}
