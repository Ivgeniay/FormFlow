using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TemplateAllowedUser> TemplateAllowedUser { get; set; }
        public DbSet<EmailPasswordAuth> EmailPasswordAuths { get; set; }
        public DbSet<FormSubscribe> FormSubscribes { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<UserContact> UserContacts { get; set; }
        public DbSet<TemplateTag> TemplateTags { get; set; }
        public DbSet<GoogleAuth> GoogleAuths { get; set; }
        public DbSet<ColorTheme> ColorThemes { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ApiToken> ApiTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
