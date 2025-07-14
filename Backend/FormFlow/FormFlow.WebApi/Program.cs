using FormFlow.Application.Interfaces;
using FormFlow.Application.Services;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Interfaces.Services;
using FormFlow.Infrastructure.Services;
using FormFlow.Persistence;
using FormFlow.Persistence.Repositories;
using FormFlow.WebApi.Common.Extensions;
using FormFlow.WebApi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nest;
using Scalar.AspNetCore;
using System.Text;
using Newtonsoft.Json.Serialization;
using FormFlow.Domain.Interfaces.Services.Jwt;

namespace FormFlow.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
            builder.Services.AddOpenApi();

            var allowedOrigins = builder.Configuration.GetValue<string>("AllowedOrigins")?.Split(',') ?? new[] { "http://localhost:3000" };

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            builder.Services.AddSignalR();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
            builder.Services.AddScoped<IFormRepository, FormRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<ILikeRepository, LikeRepository>();
            builder.Services.AddScoped<ITagRepository, TagRepository>();
            builder.Services.AddScoped<IFormSubscribeRepository, FormSubscribeRepository>();
            builder.Services.AddScoped<ITopicRepository, TopicRepository>();
            builder.Services.AddScoped<IColorThemeRepository, ColorThemeRepository>();
            builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
            builder.Services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();
            builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
            builder.Services.AddScoped<IGoogleAuthRepository, GoogleAuthRepository>();
            builder.Services.AddScoped<IUserContactRepository, UserContactRepository>();
            builder.Services.AddScoped<IEmailAuthRepository, EmailAuthRepository>();
            builder.Services.AddScoped<IApiTokenRepository, ApiTokenRepository>();


            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITemplateService, TemplateService>();
            builder.Services.AddScoped<IFormService, FormService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<ILikeService, LikeService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<IFormSubscribeService, FormSubscribeService>();
            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
            builder.Services.AddScoped<ITopicService, TopicService>();
            builder.Services.AddScoped<IColorThemeService, ColorThemeService>();
            builder.Services.AddScoped<ILanguageService, LanguageService>();
            builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();
            builder.Services.AddHttpClient<AiTemplateService>();
            builder.Services.AddScoped<IAiTemplateService, AiTemplateService>();
            builder.Services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();
            builder.Services.AddScoped<IApiTokenService, ApiTokenService>();

            builder.Services.AddScoped<IImageStorageService, GoogleCloudImageStorageService>();

            builder.Services.AddScoped<IEmailService, GmailEmailService>();
            builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();


            builder.Services.AddSingleton<IElasticClient>(ElasticConfigurating(builder));

            builder.Services.AddScoped<ISearchService, ElasticSearchService>();


            builder.Services.AddScoped<IJwtService, JwtService>();
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    RequireExpirationTime = true
                };

                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddAuthorization();

            var app = builder.Build();

#if DEBUG
            app.MapOpenApi();
            app.MapScalarApiReference();
#endif

            app.UseCors();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseMiddleware<UserContextMiddleware>();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<TemplateActivityHub>("/hubs/template-activity");

            try
            {
                Task.Run(async () =>
                {
                    await app.EnsureDB();
                    await TestDataSeeder.ExecuteTestData(app, userCount: 10, templateCount: 15, likeProbability: 0.5f);
                });
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error during database initialization: {e.Message}");
            }

            app.Run();
        }

        private static Func<IServiceProvider, IElasticClient> ElasticConfigurating(WebApplicationBuilder builder)
        {
            return provider =>
                        new ElasticClient(
                            new ConnectionSettings(
                                new Uri(builder.Configuration.GetSection("ElasticSearch:Uri").Value))
                                .DefaultIndex("templates"));
        }
    }


}
