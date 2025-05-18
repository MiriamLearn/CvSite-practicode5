using GitHubPortfolio.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubPortfolio.Service
{
    public interface IGitHubService
    {
        Task<IEnumerable<RepositoryInfo>> GetPortfolioAsync();
        Task<IEnumerable<RepositoryInfo>> SearchRepositoriesAsync(string? repositoryName = null, string? language = null, string? username = null);
        Task<DateTime> GetLatestUserActivityAsync();
    }
}
