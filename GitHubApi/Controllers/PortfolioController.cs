
using GitHubPortfolio.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GitHubPortfolio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;

        public PortfolioController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPortfolio()
        {
            var result = await _gitHubService.GetPortfolioAsync();
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchRepositories(
            [FromQuery] string? repositoryName = null,
            [FromQuery] string? language = null,
            [FromQuery] string? username = null)
        {
            var result = await _gitHubService.SearchRepositoriesAsync(repositoryName, language, username);
            return Ok(result);
        }
    }
}