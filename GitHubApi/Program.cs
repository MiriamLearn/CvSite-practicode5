using GitHubPortfolio.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scrutor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure GitHub options
builder.Services.Configure<GitHubOptions>(
    builder.Configuration.GetSection(GitHubOptions.GitHub));

// Add memory cache
builder.Services.AddMemoryCache();

// Register GitHub service with caching decorator using Scrutor
// Register GitHub service with caching decorator using Scrutor
builder.Services.AddTransient<GitHubService>();
builder.Services.AddTransient<IGitHubService, GitHubService>();
builder.Services.Decorate<IGitHubService, CachedGitHubService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();