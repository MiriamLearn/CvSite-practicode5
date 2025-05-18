using GitHubPortfolio.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using ProductHeaderValue = Octokit.ProductHeaderValue;
using Microsoft.Extensions.Options;

namespace GitHubPortfolio.Service
{
    public class GitHubService:IGitHubService
    {
        private readonly GitHubClient _client;
        private readonly string _username;

        public GitHubService(IOptions<GitHubOptions> options)
        {
            _username = options.Value.Username;

            _client = new GitHubClient(new ProductHeaderValue("GitHubPortfolio"));

            if (!string.IsNullOrEmpty(options.Value.PersonalAccessToken))
            {
                _client.Credentials = new Credentials(options.Value.PersonalAccessToken);
            }
        }
        public async Task<IEnumerable<RepositoryInfo>> GetPortfolioAsync()
        {
            var repositories = await _client.Repository.GetAllForUser(_username);
            var result = new List<RepositoryInfo>();

            foreach (var repo in repositories)
            {
                var pullRequests = await _client.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name);
                var languages = await _client.Repository.GetAllLanguages(repo.Owner.Login, repo.Name);

                DateTime lastCommitDate = repo.UpdatedAt.DateTime; // ערך ברירת מחדל

                try
                {
                    var commits = await _client.Repository.Commit.GetAll(repo.Owner.Login, repo.Name);
                    if (commits.Any())
                    {
                        lastCommitDate = commits.OrderByDescending(c => c.Commit.Author.Date).First().Commit.Author.Date.DateTime;
                    }
                }
                catch (Octokit.ApiException ex) when (ex.Message.Contains("Git Repository is empty"))
                {
                    // המאגר ריק, נשתמש בתאריך העדכון כברירת מחדל
                    Console.WriteLine($"Repository {repo.Name} is empty, using UpdatedAt date instead");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting commits for {repo.Name}: {ex.Message}");
                    // נמשיך לרפוזיטורי הבא במקרה של שגיאה
                    continue;
                }

                result.Add(new RepositoryInfo
                {
                    Name = repo.Name,
                    Description = repo.Description ?? string.Empty,
                    Url = repo.HtmlUrl,
                    Homepage = repo.Homepage ?? string.Empty,
                    Stars = repo.StargazersCount,
                    PullRequests = pullRequests.Count,
                    LastCommitDate = lastCommitDate,
                    Languages = languages.ToDictionary(l => l.Name, l => l.NumberOfBytes)
                });
            }

            return result;
        }

        public async Task<IEnumerable<RepositoryInfo>> SearchRepositoriesAsync(string? repositoryName = null, string? language = null, string? username = null)
        {
            var searchTerms = new List<string>();

            if (!string.IsNullOrEmpty(repositoryName))
                searchTerms.Add(repositoryName);

            if (!string.IsNullOrEmpty(language))
                searchTerms.Add($"language:{language}");

            if (!string.IsNullOrEmpty(username))
                searchTerms.Add($"user:{username}");

            var searchQuery = string.Join(" ", searchTerms);
            var request = new SearchRepositoriesRequest(searchQuery);

            var searchResult = await _client.Search.SearchRepo(request);
            var result = new List<RepositoryInfo>();

            foreach (var repo in searchResult.Items.Take(20)) // Limiting to 20 to avoid rate limits
            {
                var languages = await _client.Repository.GetAllLanguages(repo.Owner.Login, repo.Name);

                result.Add(new RepositoryInfo
                {
                    Name = repo.Name,
                    Description = repo.Description ?? string.Empty,
                    Url = repo.HtmlUrl,
                    Homepage = repo.Homepage ?? string.Empty,
                    Stars = repo.StargazersCount,
                    Languages = languages.ToDictionary(l => l.Name, l => l.NumberOfBytes)
                });
            }

            return result;
        }

        public async Task<DateTime> GetLatestUserActivityAsync()
        {
            var events = await _client.Activity.Events.GetAllUserPerformed(_username);
            return events.Any() ? events.Max(e => e.CreatedAt.DateTime) : DateTime.MinValue;
        }
    }
}
