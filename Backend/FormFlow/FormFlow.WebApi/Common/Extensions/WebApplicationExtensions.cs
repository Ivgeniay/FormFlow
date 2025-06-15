using FormFlow.Application.DTOs.Templates;
using FormFlow.Application.DTOs.Users;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Models.General;
using FormFlow.Domain.Models.General.QuestionDetailsModels;
using FormFlow.Persistence;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

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
                    await dbContext.Database.EnsureCreatedAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                    throw;
                }
            }
        }

        public static async Task WithDefaultColorThemes(this WebApplication source)
        {
            using (var scope = source.Services.CreateScope())
            {
                var colorThemeService = scope.ServiceProvider.GetRequiredService<IColorThemeService>();
                try
                {
                    Console.WriteLine("Ensuring default color theme...");
                    var existingThemes = await colorThemeService.GetAllAsync();
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

                        await colorThemeService.CreateAsync(lightTheme);
                        await colorThemeService.CreateAsync(darkTheme);

                        Console.WriteLine("Default color themes created successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Color themes already exist, skipping creation.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                    throw;
                }
            }
        }

        public static async Task WithDefaultLanguages(this WebApplication source)
        {
            using (var scope = source.Services.CreateScope())
            {
                var languageService = scope.ServiceProvider.GetRequiredService<ILanguageService>();
                try
                {
                    Console.WriteLine("Ensuring default languages...");

                    var existingLanguages = await languageService.GetAllAsync();
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

                        await languageService.CreateAsync(englishLanguage);
                        await languageService.CreateAsync(russianLanguage);

                        Console.WriteLine("Default languages created successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Languages already exist, skipping creation.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating default languages: {ex.Message}");
                    throw;
                }
            }
        }

        public static async Task WithDefaultTopics(this WebApplication source)
        {
            using (var scope = source.Services.CreateScope())
            {
                var topicService = scope.ServiceProvider.GetRequiredService<ITopicService>();
                try
                {
                    Console.WriteLine("Ensuring default languages...");
                    var existingTopics = await topicService.GetTopicsAsync(1, 1);
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
                            await topicService.CreateTopicAsync(topicName);
                        }
                        Console.WriteLine("Default topics created successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Topics already exist, skipping creation.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                    throw;
                }
            }
        }

        public static async Task WithUsers(this WebApplication source, int userCount)
        {
            using (var scope = source.Services.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                try
                {
                    Console.WriteLine($"Creating {userCount} test users...");

                    var random = new Random();
                    var firstNames = new[] { "John", "Jane", "Alex", "Sarah", "Mike", "Emma", "David", "Lisa", "Chris", "Anna", "Tom", "Kate", "Mark", "Amy", "Paul", "Lucy" };
                    var lastNames = new[] { "Smith", "Johnson", "Brown", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Garcia", "Martinez", "Robinson", "Clark", "Rodriguez", "Lewis" };

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

                    Console.WriteLine($"Test users creation completed. Created: {createdCount}/{userCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating test users: {ex.Message}");
                    throw;
                }
            }
        }

        public static async Task WithTemplates(this WebApplication source, int templateCount)
        {
            using (var scope = source.Services.CreateScope())
            {
                var templateService = scope.ServiceProvider.GetRequiredService<ITemplateService>();
                var topicService = scope.ServiceProvider.GetRequiredService<ITopicService>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                try
                {
                    Console.WriteLine($"Creating {templateCount} test templates...");

                    var topics = await topicService.GetTopicsAsync(100);
                    if (topics.Count == 0)
                    {
                        Console.WriteLine("No topics found, skipping template creation.");
                        return;
                    }

                    var users = await userService.GetUsersPagedAsync(1, int.MaxValue);
                    if (users.Data.Count == 0)
                    {
                        Console.WriteLine("No users found, skipping template creation.");
                        return;
                    }

                    var random = new Random();
                    var templateTitles = new[]
                    {
                        "Customer Satisfaction Survey", "Job Application Form", "Event Registration", "Course Evaluation",
                        "Product Feedback", "Employee Survey", "Market Research", "Contact Information", "Booking Form",
                        "Newsletter Subscription", "Bug Report", "Feature Request", "Training Assessment", "Health Survey",
                        "Travel Questionnaire", "Food Preferences", "Skill Assessment", "Performance Review"
                    };

                    var descriptions = new[]
                    {
                        "Help us improve our services by sharing your feedback",
                        "Please fill out this form to apply for the position",
                        "Register for our upcoming event",
                        "Evaluate the course content and instructor",
                        "Share your thoughts about our product",
                        "We value your opinion as our employee",
                        "Participate in our market research study",
                        "Please provide your contact details",
                        "Book your appointment or service",
                        "Stay updated with our latest news"
                    };

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
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating test templates: {ex.Message}");
                    throw;
                }
            }
        }

        public static async Task WithLikes(this WebApplication source, float probability)
        {
            using (var scope = source.Services.CreateScope())
            {
                var likeService = scope.ServiceProvider.GetRequiredService<ILikeService>();
                var templateService = scope.ServiceProvider.GetRequiredService<ITemplateService>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                try
                {
                    Console.WriteLine($"Creating test likes with {probability:P} probability...");

                    var templates = await templateService.GetPublicTemplatesPagedAsync(1, int.MaxValue);
                    if (templates.Data.Count == 0)
                    {
                        Console.WriteLine("No templates found, skipping likes creation.");
                        return;
                    }

                    var users = await userService.GetUsersPagedAsync(1, int.MaxValue);
                    if (users.Data.Count == 0)
                    {
                        Console.WriteLine("No users found, skipping likes creation.");
                        return;
                    }

                    var random = new Random();
                    var likesCreated = 0;
                    var totalPossibleLikes = users.Data.Count * templates.Data.Count;

                    foreach (var user in users.Data)
                    {
                        foreach (var template in templates.Data)
                        {
                            if (random.NextSingle() < probability)
                            {
                                try
                                {
                                    var success = await likeService.AddLikeAsync(user.Id, template.Id);
                                    if (success)
                                    {
                                        likesCreated++;
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }

                    Console.WriteLine($"Test likes creation completed. Created: {likesCreated}/{totalPossibleLikes} possible likes");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating test likes: {ex.Message}");
                    throw;
                }
            }
        }

        private static List<string> GenerateRandomTags(Random random)
        {
            var allTags = new[] { "survey", "feedback", "form", "quiz", "registration", "evaluation", "assessment", "questionnaire" };
            var count = random.Next(1, 4);
            return allTags.OrderBy(x => random.Next()).Take(count).ToList();
        }

        private static List<QuestionDto> GenerateRandomQuestions(Random random)
        {
            var questions = new List<QuestionDto>();
            var questionCount = random.Next(3, 8);

            for (var i = 0; i < questionCount; i++)
            {
                var questionType = (QuestionType)random.Next(1, 8);
                var questionData = GenerateQuestionData(questionType, i, random);

                questions.Add(new QuestionDto
                {
                    Id = Guid.Empty,
                    Order = i + 1,
                    ShowInResults = random.Next(100) < 70,
                    IsRequired = random.Next(100) < 60,
                    Data = JsonConvert.SerializeObject(questionData),
                    CreatedAt = DateTime.UtcNow
                });
            }

            return questions;
        }

        private static QuestionDetails GenerateQuestionData(QuestionType type, int index, Random random)
        {
            var titles = new[]
            {
                "What is your name?", "How would you rate our service?", "Which option do you prefer?",
                "Please provide additional comments", "When would you like to schedule?", "How did you hear about us?",
                "What is your experience level?", "Which features are most important?", "Rate your satisfaction",
                "Select all that apply", "Describe your requirements", "What is your budget range?"
            };

            var title = titles[random.Next(titles.Length)] + $" (Q{index + 1})";

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
