using FormFlow.Application.Interfaces;
using FormFlow.Application.Services;
using FormFlow.Persistence;

namespace FormFlow.WebApi.Common.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication EnsureDB(this WebApplication source)
        {
            using (var scope = source.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    Console.WriteLine("Ensuring database is created and up to date...");
                    dbContext.Database.EnsureCreated();
                    return source;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                    throw;
                }
            }
        }

        public static WebApplication WithDefaultColorThemes(this WebApplication source)
        {
            using (var scope = source.Services.CreateScope())
            {
                var colorThemeSerivice = scope.ServiceProvider.GetRequiredService<IColorThemeService>();
                try
                {
                    Console.WriteLine("Ensuring default color theme...");
                    
                    return source;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                    throw;
                }
            }
        }

        public static WebApplication WithDefaultLanguages(this WebApplication source)
        {
            using (var scope = source.Services.CreateScope())
            {
                var languageSerivice = scope.ServiceProvider.GetRequiredService<ILanguageService>();
                try
                {
                    Console.WriteLine("Ensuring default languages...");

                    return source;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                    throw;
                }
            }
        }

        public static WebApplication WithDefaultTopics(this WebApplication source)
        {
            using (var scope = source.Services.CreateScope())
            {
                var topicSerivice = scope.ServiceProvider.GetRequiredService<ITopicService>();
                try
                {
                    Console.WriteLine("Ensuring default languages...");

                    return source;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
