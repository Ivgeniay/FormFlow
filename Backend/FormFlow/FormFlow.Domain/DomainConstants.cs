public static class DomainConstants
{
    public static class Validation
    {
        public const int UserNameMinLength = 1;
        public const int UserNameMaxLength = 50;
        public const int EmailMaxLength = 100;
        public const int PasswordMinLength = 3;
        public const int PasswordMaxLength = 100;

        public const int TemplateNameMinLength = 3;
        public const int TemplateNameMaxLength = 200;
        public const int TemplateDescriptionMaxLength = 2000;

        public const int QuestionTitleMinLength = 3;
        public const int QuestionTitleMaxLength = 300;
        public const int QuestionDescriptionMaxLength = 1000;

        public const int CommentMinLength = 1;
        public const int CommentMaxLength = 1000;

        public const int TagNameMinLength = 2;
        public const int TagNameMaxLength = 50;

        public const int ContactValueMaxLength = 200;

        public const int MaxQuestionsPerTemplate = 300;
        public const int MaxTagsPerTemplate = 20;
        public const int MaxAllowedUsersPerTemplate = 100;
    }

    public static class Database
    {
        public static class TableNames
        {
            public const string Users = "users";
            public const string Templates = "templates";
            public const string Questions = "questions";
            public const string Forms = "forms";
            public const string Comments = "comments";
            public const string Likes = "likes";
            public const string Tags = "tags";
            public const string UserContacts = "user_contacts";
            public const string EmailPasswordAuths = "email_password_auths";
            public const string GoogleAuths = "google_auths";
            public const string TemplateTags = "template_tags";
            public const string TemplateAllowedUsers = "template_allowed_users";
        }

        public static class ColumnNames
        {
            public const string Id = "id";
            public const string CreatedAt = "created_at";
            public const string UpdatedAt = "updated_at";
            public const string IsDeleted = "is_deleted";
            public const string UserId = "user_id";
            public const string TemplateId = "template_id";
            public const string FormId = "form_id";
            public const string QuestionId = "question_id";
            public const string TagId = "tag_id";
        }

        public static class IndexNames
        {
            public const string UsersEmailIndex = "idx_users_email";
            public const string LikesTemplateUserIndex = "idx_likes_template_user";
            public const string TagsNameIndex = "idx_tags_name";
        }
    }

    public static class SignalR
    {
        public static class HubNames
        {
            public const string CommentsHub = "/hubs/comments";
        }

        public static class Events
        {
            public const string NewComment = nameof(NewComment);
            public const string LikeUpdate = nameof(LikeUpdate);
            public const string UserJoined = nameof(UserJoined);
            public const string UserLeft = nameof(UserLeft);
            public const string UserCount = nameof(UserCount);
        }

        public static class Methods
        {
            public const string JoinTemplate = nameof(JoinTemplate);
            public const string LeaveTemplate = nameof(LeaveTemplate);
            public const string AddComment = nameof(AddComment);
            public const string ToggleLike = nameof(ToggleLike);
        }

        public static class Groups
        {
            public static string TemplateGroup(Guid templateId) => $"template_{templateId}";
        }
    }

    public static class Pagination
    {
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;
        public const int MinPageSize = 5;
    }

    public static class Search
    {
        public const int DefaultSearchLimit = 10;
        public const int MaxSearchLimit = 50;
        public const int MinQueryLength = 2;
        public const int MaxQueryLength = 200;
    }

    public static class Images
    {
        public const long MaxFileSizeBytes = 5 * 1024 * 1024;
        public const int MaxWidthPixels = 1920;
        public const int MaxHeightPixels = 1080;
        public static readonly string[] AllowedContentTypes =
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/webp"
        };
    }

    public static class Email
    {
        public const int SubjectMaxLength = 200;
        public const int MaxRecipientsCount = 10;
    }
}