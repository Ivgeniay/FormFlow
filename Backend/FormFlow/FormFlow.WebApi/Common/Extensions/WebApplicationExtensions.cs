using FormFlow.Application.DTOs.Templates;
using FormFlow.Application.DTOs.Users;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Models.General;
using FormFlow.Domain.Models.General.QuestionDetailsModels;
using FormFlow.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FormFlow.WebApi.Common.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task EnsureDB(this WebApplication source)
        {
            using (var scope = source.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    Console.WriteLine("Ensuring database is created and up to date...");
                    //await dbContext.Database.EnsureDeletedAsync();
                    await dbContext.Database.EnsureCreatedAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                    throw;
                }
            }
        }

    }

    public static class TestDataSeeder
    {
        private static readonly string[] firstNames = { "John", "Jane", "Alex", "Sarah", "Mike", "Emma", "David", "Lisa", "Chris", "Anna", "Tom", "Kate", "Mark", "Amy", "Paul", "Lucy" };
        private static readonly string[] lastNames = { "Smith", "Johnson", "Brown", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Garcia", "Martinez", "Robinson", "Clark", "Rodriguez", "Lewis" };
        private static readonly string[] templateTitles = { "Customer Satisfaction Survey", "Job Application Form", "Event Registration", "Course Evaluation",
                    "Product Feedback", "Employee Survey", "Market Research", "Contact Information", "Booking Form",
                    "Newsletter Subscription", "Bug Report", "Feature Request", "Training Assessment", "Health Survey",
                    "Travel Questionnaire", "Food Preferences", "Skill Assessment", "Performance Review" };
        private static readonly string[] descriptions = { "Help us improve our services by sharing your feedback",
                    "Please fill out this form to apply for the position",
                    "Register for our upcoming event",
                    "Evaluate the course content and instructor",
                    "Share your thoughts about our product",
                    "We value your opinion as our employee",
                    "Participate in our market research study",
                    "Please provide your contact details",
                    "Book your appointment or service",
                    "Stay updated with our latest news" };
        private static readonly string[] allTags = { "survey", "feedback", "form", "quiz", "registration", "evaluation", "assessment", "questionnaire" };
        private static readonly string[] qTitles = { "What is your name?", "How would you rate our service?", "Which option do you prefer?",
            "Please provide additional comments", "When would you like to schedule?", "How did you hear about us?",
            "What is your experience level?", "Which features are most important?", "Rate your satisfaction",
            "Select all that apply", "Describe your requirements", "What is your budget range?" };
        private static readonly string[] defaultTopics = { "Education", "Quiz", "Survey", "Poll", "Feedback", "Registration", "Application", "Other" };

        public static async Task<bool> ExecuteTestData(WebApplication app, int userCount = 10, int templateCount = 15, float likeProbability = 0.5f)
        {
            using (var scope = app.Services.CreateScope())
            {
                var steps = new List<Func<IServiceScope, Task<bool>>>
                {
                    CreateDefaultColorThemes,
                    CreateDefaultLanguages,
                    CreateDefaultTopics,
                    (scope) => CreateUsers(scope, userCount),
                    (scope) => CreateTemplates(scope, templateCount),
                    //(scope) => CreateLikes(scope, likeProbability)
                };

                foreach (var step in steps)
                {
                    try
                    {
                        var success = await step(scope);
                        if (!success)
                        {
                            Console.WriteLine("Test data execution stopped due to error.");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Critical error during test data execution: {ex.Message}");
                        return false;
                    }
                }

                Console.WriteLine("All test data created successfully.");
                return true;
            }
        }

        private static async Task<bool> CreateDefaultColorThemes(IServiceScope scope)
        {
            var colorThemeService = scope.ServiceProvider.GetRequiredService<IColorThemeService>();

            try
            {
                Console.WriteLine("Creating default color themes...");
                var existingThemes = await colorThemeService.GetAllAsync();

                if (existingThemes.Any())
                {
                    Console.WriteLine("Color themes already exist, skipping creation.");
                    return true;
                }

                var lightTheme = new ColorTheme
                {
                    Name = "Light",
                    CssClass = "theme-light",
                    ColorVariables = @"{
""--primary-color"": ""#3b82f6"",
""--background-color"": ""#ffffff"",
""--surface-color"": ""#f8fafc"",
""--text-color"": ""#1e293b"",
""--text-muted-color"": ""#64748b"",
""--border-color"": ""#e2e8f0"",
""--success-color"": ""#10b981"",
""--warning-color"": ""#f59e0b"",
""--error-color"": ""#ef4444""
}",
                    IsDefault = true,
                    IsActive = true
                };

                var darkTheme = new ColorTheme
                {
                    Name = "Dark",
                    CssClass = "theme-dark",
                    ColorVariables = @"{
""--primary-color"": ""#60a5fa"",
""--background-color"": ""#0f172a"",
""--surface-color"": ""#1e293b"",
""--text-color"": ""#f1f5f9"",
""--text-muted-color"": ""#94a3b8"",
""--border-color"": ""#334155"",
""--success-color"": ""#34d399"",
""--warning-color"": ""#fbbf24"",
""--error-color"": ""#f87171""
}",
                    IsDefault = false,
                    IsActive = true
                };

                var purpleTheme = new ColorTheme
                {
                    Name = "Purple",
                    CssClass = "theme-purple",
                    ColorVariables = @"{
""--primary-color"": ""#8b5cf6"",
""--background-color"": ""#faf5ff"",
""--surface-color"": ""#f3e8ff"",
""--text-color"": ""#581c87"",
""--text-muted-color"": ""#7c3aed"",
""--border-color"": ""#ddd6fe"",
""--success-color"": ""#10b981"",
""--warning-color"": ""#f59e0b"",
""--error-color"": ""#ef4444""
}",
                    IsDefault = false,
                    IsActive = true
                };

                var orangeTheme = new ColorTheme
                {
                    Name = "Orange",
                    CssClass = "theme-orange",
                    ColorVariables = @"{
""--primary-color"": ""#ea580c"",
""--background-color"": ""#fff7ed"",
""--surface-color"": ""#fed7aa"",
""--text-color"": ""#9a3412"",
""--text-muted-color"": ""#c2410c"",
""--border-color"": ""#fdba74"",
""--success-color"": ""#10b981"",
""--warning-color"": ""#f59e0b"",
""--error-color"": ""#ef4444""
}",
                    IsDefault = false,
                    IsActive = true
                };

                var pinkTheme = new ColorTheme
                {
                    Name = "Pink",
                    CssClass = "theme-pink",
                    ColorVariables = @"{
""--primary-color"": ""#ec4899"",
""--background-color"": ""#fdf2f8"",
""--surface-color"": ""#fce7f3"",
""--text-color"": ""#831843"",
""--text-muted-color"": ""#be185d"",
""--border-color"": ""#f9a8d4"",
""--success-color"": ""#10b981"",
""--warning-color"": ""#f59e0b"",
""--error-color"": ""#ef4444""
}",
                    IsDefault = false,
                    IsActive = true
                };

                await colorThemeService.CreateAsync(lightTheme);
                await colorThemeService.CreateAsync(darkTheme);
                await colorThemeService.CreateAsync(purpleTheme);
                await colorThemeService.CreateAsync(orangeTheme);
                await colorThemeService.CreateAsync(pinkTheme);

                Console.WriteLine("Default color themes created successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating color themes: {ex.Message}");
                return false;
            }
        }

        private static async Task<bool> CreateDefaultLanguages(IServiceScope scope)
        {
            var languageService = scope.ServiceProvider.GetRequiredService<ILanguageService>();

            try
            {
                Console.WriteLine("Creating default languages...");
                var existingLanguages = await languageService.GetAllAsync();

                if (existingLanguages.Any())
                {
                    Console.WriteLine("Languages already exist, skipping creation.");
                    return true;
                }

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

                await languageService.CreateAsync(englishLanguage);
                await languageService.CreateAsync(russianLanguage);

                Console.WriteLine("Default languages created successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating languages: {ex.Message}");
                return false;
            }
        }

        private static async Task<bool> CreateDefaultTopics(IServiceScope scope)
        {
            var topicService = scope.ServiceProvider.GetRequiredService<ITopicService>();

            try
            {
                Console.WriteLine("Creating default topics...");
                var existingTopics = await topicService.GetTopicsAsync(1, 1);

                if (existingTopics.Pagination.TotalCount > 0)
                {
                    Console.WriteLine("Topics already exist, skipping creation.");
                    return true;
                }

                foreach (var topicName in defaultTopics)
                    await topicService.CreateTopicAsync(topicName);

                Console.WriteLine("Default topics created successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating topics: {ex.Message}");
                return false;
            }
        }

        private static async Task<bool> CreateUsers(IServiceScope scope, int userCount)
        {
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            try
            {
                var users = await userService.GetUsersPagedAsync(1, 10);
                if (users.Pagination.TotalCount > 0)
                {
                    Console.WriteLine("Users already exist, skipping creation.");
                    return true;
                }

                Console.WriteLine($"Creating {userCount} test users...");

                var random = new Random();
                var createdCount = 0;
                var attempts = 0;
                var maxAttempts = userCount * 3;

                while (createdCount < userCount && attempts < maxAttempts)
                {
                    attempts++;

                    var firstName = firstNames[random.Next(firstNames.Length)];
                    var lastName = lastNames[random.Next(lastNames.Length)];
                    var userName = $"{firstName.ToLower()}{lastName.ToLower()}{random.Next(1, 999)}";
                    var email = $"{firstName.ToLower()}.{lastName.ToLower()}{random.Next(1, 999)}@testmail.com";

                    if (await userService.UserNameExistsAsync(userName) ||
                        await userService.EmailExistsAsync(email))
                    {
                        continue;
                    }

                    var registerRequest = new RegisterUserRequest
                    {
                        UserName = userName,
                        Email = email,
                        Password = "password"
                    };

                    var result = await userService.RegisterUserAsync(registerRequest);
                    if (result.IsSuccess)
                    {
                        createdCount++;
                        if (createdCount % 10 == 0)
                        {
                            Console.WriteLine($"Created {createdCount} users...");
                        }
                    }
                }

                if (createdCount < userCount)
                {
                    Console.WriteLine($"Could only create {createdCount} out of {userCount} users.");
                    return false;
                }

                Console.WriteLine($"Test users creation completed. Created: {createdCount}/{userCount}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating test users: {ex.Message}");
                return false;
            }
        }

        private static async Task<bool> CreateTemplates(IServiceScope scope, int templateCount)
        {
            var templateService = scope.ServiceProvider.GetRequiredService<ITemplateService>();
            var topicService = scope.ServiceProvider.GetRequiredService<ITopicService>();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            try
            {
                var templates = await templateService.GetPublicTemplatesPagedAsync(1, 10);
                if (templates.Data.Count > 0)
                {
                    Console.WriteLine("Templates already exist, skipping creation.");
                    return true;
                }

                Console.WriteLine($"Creating {templateCount} test templates...");

                var topics = await topicService.GetTopicsAsync(100);
                if (topics.Count == 0)
                {
                    Console.WriteLine("No topics found, skipping template creation.");
                    return false;
                }

                var users = await userService.GetUsersPagedAsync(1, 20);
                if (users.Data.Count == 0)
                {
                    Console.WriteLine("No users found, skipping template creation.");
                    return false;
                }

                var random = new Random();

                for (var i = 0; i < templateCount; i++)
                {
                    var randomUser = users.Data[random.Next(users.Data.Count)];
                    var randomTopic = topics[random.Next(topics.Count)];
                    var title = templateTitles[random.Next(templateTitles.Length)];
                    var description = descriptions[random.Next(descriptions.Length)];

                    var createRequest = new CreateTemplateRequest
                    {
                        Title = title,
                        Description = description,
                        TopicId = randomTopic.Id,
                        AccessType = TemplateAccess.Public,
                        Tags = GenerateRandomTags(random),
                        AllowedUserIds = new List<Guid>()
                    };

                    var template = await templateService.CreateTemplateAsync(createRequest, randomUser.Id);

                    var updateRequest = new UpdateTemplateRequest
                    {
                        Id = template.Id,
                        Title = template.Title,
                        Description = template.Description,
                        TopicId = template.TopicId.Value,
                        AccessType = template.AccessType,
                        Questions = GenerateRandomQuestions(random),
                        Tags = createRequest.Tags,
                        AllowedUserIds = new List<Guid>()
                    };

                    await templateService.UpdateTemplateAsync(updateRequest, randomUser.Id);
                    await templateService.PublishTemplateAsync(template.Id, randomUser.Id);

                    if ((i + 1) % 5 == 0)
                    {
                        Console.WriteLine($"Created {i + 1} templates...");
                    }
                }

                Console.WriteLine($"Test templates creation completed: {templateCount}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating test templates: {ex.Message}");
                return false;
            }
        }

        private static async Task<bool> CreateLikes(IServiceScope scope, float probability)
        {
            var likeService = scope.ServiceProvider.GetRequiredService<ILikeService>();
            var templateService = scope.ServiceProvider.GetRequiredService<ITemplateService>();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            try
            {
                Console.WriteLine($"Creating test likes with {probability:P} probability...");

                var templatesResult = await templateService.GetPublicTemplatesPagedAsync(1, 1000);
                if (templatesResult.Data.Count == 0)
                {
                    Console.WriteLine("No templates found, skipping likes creation.");
                    return false;
                }

                var usersResult = await userService.GetUsersPagedAsync(1, 1000);
                if (usersResult.Data.Count == 0)
                {
                    Console.WriteLine("No users found, skipping likes creation.");
                    return false;
                }

                var random = new Random();
                var likesCreated = 0;
                var likesDropped = 0;
                var totalPossibleLikes = usersResult.Data.Count * templatesResult.Data.Count;
                var errors = 0;

                foreach (var user in usersResult.Data)
                {
                    foreach (var template in templatesResult.Data)
                    {
                        if (random.NextSingle() < probability)
                        {
                            try
                            {
                                var success = await likeService.ToggleLikeAsync(user.Id, template.Id);
                                if (success.IsLiked)
                                {
                                    likesCreated++;
                                }
                                else
                                {
                                    likesDropped++;
                                }
                            }
                            catch (Exception ex)
                            {
                                errors++;
                                if (errors > 10)
                                {
                                    Console.WriteLine($"Too many errors creating likes: {ex.Message}");
                                    return false;
                                }
                            }
                        }
                    }
                }

                Console.WriteLine($"Test likes creation completed. Created: {likesCreated}/{totalPossibleLikes} possible likes. Likes dropped: {likesDropped}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating test likes: {ex.Message}");
                return false;
            }
        }

        private static List<string> GenerateRandomTags(Random random)
        {
            var count = random.Next(1, 4);
            return allTags.OrderBy(x => random.Next()).Take(count).ToList();
        }

        private static List<UpdateQuestionDto> GenerateRandomQuestions(Random random)
        {
            var questions = new List<UpdateQuestionDto>();
            var questionCount = random.Next(3, 8);
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };


            for (var i = 0; i < questionCount; i++)
            {
                var questionType = (QuestionType)random.Next(1, 8);
                var questionData = GenerateQuestionData(questionType, i, random);

                questions.Add(new UpdateQuestionDto
                {
                    Id = "",
                    IsNewQuestion = true,
                    Order = i + 1,
                    ShowInResults = random.Next(100) < 70,
                    IsRequired = random.Next(100) < 60,
                    Data = JsonConvert.SerializeObject(questionData, jsonSettings),
                    CreatedAt = DateTime.UtcNow
                });
            }

            return questions;
        }

        private static QuestionDetails GenerateQuestionData(QuestionType type, int index, Random random)
        {
            var title = qTitles[random.Next(qTitles.Length)] + $" (Q{index + 1})";

            return type switch
            {
                QuestionType.ShortText => new ShortTextDetails { Type = type, Title = title, Description = "Please enter your response", MaxLength = 100 },
                QuestionType.LongText => new LongTextDetails { Type = type, Title = title, Description = "Please provide detailed information", MaxLength = 500 },
                QuestionType.SingleChoice => new SingleChoiceDetails { Type = type, Title = title, Description = "Select one option", Options = new List<string> { "Option A", "Option B", "Option C", "Other" } },
                QuestionType.MultipleChoice => new MultipleChoiceDetails { Type = type, Title = title, Description = "Select all that apply", Options = new List<string> { "Choice 1", "Choice 2", "Choice 3", "Choice 4" }, MaxSelections = 3 },
                QuestionType.Dropdown => new DropdownDetails { Type = type, Title = title, Description = "Choose from dropdown", Options = new List<string> { "Item 1", "Item 2", "Item 3" } },
                QuestionType.Scale => new ScaleDetails { Type = type, Title = title, Description = "Rate on a scale", MinValue = 1, MaxValue = 5, MinLabel = "Poor", MaxLabel = "Excellent" },
                QuestionType.Rating => new RatingDetails { Type = type, Title = title, Description = "Give your rating", MaxRating = 5 },
                _ => new ShortTextDetails { Type = QuestionType.ShortText, Title = title, Description = "Default question" }
            };
        }
    }

}
