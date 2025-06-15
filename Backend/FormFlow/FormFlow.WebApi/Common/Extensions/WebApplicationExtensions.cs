using FormFlow.Application.Interfaces;
using FormFlow.Application.Services;
using FormFlow.Domain.Models.General;
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
                var colorThemeService = scope.ServiceProvider.GetRequiredService<IColorThemeService>();
                try
                {
                    Console.WriteLine("Ensuring default color theme...");
                    var existingThemes = colorThemeService.GetAllAsync().GetAwaiter().GetResult();
                    if (!existingThemes.Any())
                    {
                        var lightTheme = new ColorTheme
                        {
                            Name = "Light",
                            CssClass = "theme-light",
                            PrimaryColor = "#007bff",
                            IsDefault = true,
                            IsActive = true
                        };

                        var darkTheme = new ColorTheme
                        {
                            Name = "Dark",
                            CssClass = "theme-dark",
                            PrimaryColor = "#6f42c1",
                            IsDefault = false,
                            IsActive = true
                        };

                        colorThemeService.CreateAsync(lightTheme).GetAwaiter().GetResult();
                        colorThemeService.CreateAsync(darkTheme).GetAwaiter().GetResult();

                        Console.WriteLine("Default color themes created successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Color themes already exist, skipping creation.");
                    }

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
                var languageService = scope.ServiceProvider.GetRequiredService<ILanguageService>();
                try
                {
                    Console.WriteLine("Ensuring default languages...");

                    var existingLanguages = languageService.GetAllAsync().GetAwaiter().GetResult();
                    if (!existingLanguages.Any())
                    {
                        var englishLanguage = new Language
                        {
                            Code = "en-US",
                            ShortCode = "en",
                            Name = "English",
                            Region = "United States",
                            IconURL = null,
                            IsDefault = true,
                            IsActive = true
                        };

                        var russianLanguage = new Language
                        {
                            Code = "ru-RU",
                            ShortCode = "ru",
                            Name = "Русский",
                            Region = "Russia",
                            IconURL = null,
                            IsDefault = false,
                            IsActive = true
                        };

                        languageService.CreateAsync(englishLanguage).GetAwaiter().GetResult();
                        languageService.CreateAsync(russianLanguage).GetAwaiter().GetResult();

                        Console.WriteLine("Default languages created successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Languages already exist, skipping creation.");
                    }

                    return source;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating default languages: {ex.Message}");
                    throw;
                }
            }
        }

        public static WebApplication WithDefaultTopics(this WebApplication source)
        {
            using (var scope = source.Services.CreateScope())
            {
                var topicService = scope.ServiceProvider.GetRequiredService<ITopicService>();
                try
                {
                    Console.WriteLine("Ensuring default languages...");
                    var existingTopics = topicService.GetTopicsAsync(1, 1).GetAwaiter().GetResult();
                    if (existingTopics.Pagination.TotalCount == 0)
                    {
                        var defaultTopics = new[]
                        {
                            "Education",
                            "Quiz",
                            "Survey",
                            "Poll",
                            "Feedback",
                            "Registration",
                            "Application",
                            "Other"
                        };

                        foreach (var topicName in defaultTopics)
                        {
                            topicService.CreateTopicAsync(topicName).GetAwaiter().GetResult();
                        }
                        Console.WriteLine("Default topics created successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Topics already exist, skipping creation.");
                    }

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
