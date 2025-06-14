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

namespace FormFlow.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
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


            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITemplateService, TemplateService>();
            builder.Services.AddScoped<IFormService, FormService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<ILikeService, LikeService>();
            builder.Services.AddScoped<ITagService, TagService>();

            builder.Services.AddScoped<IImageStorageService, GoogleCloudImageStorageService>();

            builder.Services.AddScoped<IEmailService, GmailEmailService>();

            builder.Services.AddSingleton<IElasticClient>(provider => 
            new ElasticClient(
                new ConnectionSettings(
                    new Uri(builder.Configuration.GetSection("ElasticSearch:Uri").Value))
                    .DefaultIndex("templates")));

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
            app.EnsureDB();
            app.UseCors();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseMiddleware<UserContextMiddleware>();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<TemplateActivityHub>("/hubs/template-activity");

            app.Run();
        }
    }


}
