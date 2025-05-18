using GitHubPortfolio.Service.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubPortfolio.Service
{
        public class CachedGitHubService : IGitHubService
        {
            private readonly IGitHubService _gitHubService;
            private readonly IMemoryCache _cache;
            private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);
            private const string PortfolioCacheKey = "portfolio";
            private const string LastActivityCacheKey = "lastActivity";
            private DateTime _lastCachedActivityTime = DateTime.MinValue;

            public CachedGitHubService(IGitHubService gitHubService, IMemoryCache cache)
            {
                _gitHubService = gitHubService;
                _cache = cache;
            }

            public async Task<IEnumerable<RepositoryInfo>> GetPortfolioAsync()
            {
                // Check if there's new activity since last cache
                var latestActivity = await GetLatestUserActivityAsync();

                if (latestActivity > _lastCachedActivityTime)
                {
                    _cache.Remove(PortfolioCacheKey);
                    _lastCachedActivityTime = latestActivity;
                }

                return await _cache.GetOrCreateAsync(PortfolioCacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
                    return await _gitHubService.GetPortfolioAsync();
                });
            }

            public async Task<IEnumerable<RepositoryInfo>> SearchRepositoriesAsync(string? repositoryName = null, string? language = null, string? username = null)
            {
                // Don't cache search results as they can vary widely
                return await _gitHubService.SearchRepositoriesAsync(repositoryName, language, username);
            }

            public async Task<DateTime> GetLatestUserActivityAsync()
            {
                return await _cache.GetOrCreateAsync(LastActivityCacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return await _gitHubService.GetLatestUserActivityAsync();
                });
            }
        }
}
