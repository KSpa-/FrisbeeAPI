using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FrisbeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public TeamsController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // GET: api/teams/scrape
        [HttpGet("scrape")]
        public async Task<IActionResult> ScrapeAndStoreTeams()
        {
            try
            {
                var pythonScriptPath = @"USAUScraper\teamScraper.py";  

                // Set up the process to run the Python script
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "python",  
                    Arguments = pythonScriptPath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Execute the Python script
                using (var process = Process.Start(processStartInfo))
                using (var reader = process.StandardOutput)
                {
                    var result = await reader.ReadToEndAsync();
                    process.WaitForExit();

                    // Assuming the scraper returns team names separated by newlines 
                    var teams = result.Split('\n');

                    // Resolve the AppDbContext from the service provider
                    var dbContext = _serviceProvider.GetRequiredService<AppDbContext>();

                    // Add each team to the database
                    foreach (var teamName in teams)
                    {
                        if (!string.IsNullOrEmpty(teamName))
                        {
                            var team = new Team
                            {
                                Name = teamName.Trim()
                            };

                            await dbContext.Teams.AddAsync(team);
                        }
                    }

                    // Save changes to the database
                    await dbContext.SaveChangesAsync();

                    return Ok("Teams scraped and stored successfully");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}