using FormFlow.Application.Interfaces;
using FormFlow.Application.Services;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Interfaces.Services;
using FormFlow.Infrastructure.Services;
using FormFlow.Persistence;
using FormFlow.Persistence.Repositories;
using FormFlow.WebApi.Extensions;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace FormFlow.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

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

            builder.Services.AddSingleton<IElasticClient>(provider =>
            {
                var uri = builder.Configuration.GetSection("ElasticSearch:Uri").Value;
                var settings = new ConnectionSettings(new Uri(uri))
                    .DefaultIndex("templates");
                return new ElasticClient(settings);
            });
            builder.Services.AddScoped<ISearchService, ElasticSearchService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.EnsureDB();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
