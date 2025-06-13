public static class DomainConstants
{
    public static class Validation
    {
        public const int UserNameMinLength = 1;
        public const int UserNameMaxLength = 50;
        public const int EmailMaxLength = 100;
        public const int PasswordMinLength = 3;
        public const int PasswordMaxLength = 100;
        public const int PasswordHashMaxLength = 255;
        public const int GoogleIdMaxLength = 250;
        public const int RefreshTokenMaxLength = 500;

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

        public const int ImageUrlMaxLength = 500;
    }

    public static class Database
    {
        public static class TableNames
        {
            public const string Users = nameof(Users);
            public const string Templates = nameof(Templates);
            public const string Questions = nameof(Questions);
            public const string Forms = nameof(Forms);
            public const string Comments = nameof(Comments);
            public const string Likes = nameof(Likes);
            public const string Tags = nameof(Tags);
            public const string UserContacts = nameof(UserContacts);
            public const string EmailPasswordAuths = nameof(EmailPasswordAuths);
            public const string GoogleAuths = nameof(GoogleAuths);
            public const string TemplateTags = nameof(TemplateTags);
            public const string TemplateAllowedUsers = nameof(TemplateAllowedUsers);
        }

        public static class ColumnTypes
        {
            public const string JsonColumnType = "nvarchar(max)";
            
        }

        public static class ColumnNames
        {
            public const string Id = "id";
            public const string UserId = "user_id";
            public const string LikeId = "like_id";
            public const string EmailPasswordAuthId = "email_password_auth_id";
            public const string CommentId = "comment_id";
            public const string UserContactId = "user_contact_id";
            public const string TemplateId = "template_id";
            public const string QuestionId = "question_id";
            public const string FormId = "form_id";
            public const string TagId = "tag_id";
            public const string TemplateTagId = "template_tag_id";
            public const string GoogleAuthId = "google_auth_id";
            public const string GoogleId = "google_id";
            public const string TemplateAllowedUserId = "template_allowed_user_id";

            public const string Email = "email";
            public const string PasswordHash = "password_hash";
            public const string RefreshToken = "refresh_token";
            public const string TokenExpiry = "token_expiry";

            public const string CreatedAt = "created_at";
            public const string UpdatedAt = "updated_at";
            public const string IsDeleted = "is_deleted";

            public const string ContactType = "contact_type";
            public const string ContactValue = "contact_value";
            public const string IsPrimary = "is_primary";

            public const string CommentContent = "content";

            public const string AuthorId = "author_id";
            public const string TemplateTitle = "title";
            public const string TemplateDescription = "description";
            public const string ImageUrl = "image_url";
            public const string AccessType = "access_type";
            public const string IsArchived = "is_archived";
            public const string IsPublished = "is_published";
            public const string Version = "version";
            public const string IsCurrentVersion = "is_current_version";
            public const string BaseTemplateId = "base_template_id";
            public const string PreviousVersionId = "previous_version_id";
            public const string QuestionOrder = "order";
            public const string ShowInResults = "show_in_results";
            public const string IsRequired = "is_required";
            public const string QuestionData = "data";

            public const string AnswersData = "answers_data";
            public const string SubmittedAt = "submitted_at";
            public const string TemplateVersion = "template_version";

            public const string TagName = "name";
            public const string UsageCount = "usage_count";

        }

        public static class DefaultValues
        {
            public const int VersionDefault = 1;
            public const int UsageCountDefault = 0;
            public const bool IsDeletedDefault = false;
            public const bool IsArchivedDefault = false;
            public const bool IsPublishedDefault = false;
            public const bool IsCurrentVersionDefault = true;
            public const bool ShowInResultsDefault = false;
            public const bool IsRequiredDefault = false;
            public const bool IsPrimaryDefault = false;
        }

        public static class IndexNames
        {
            public const string UsersuserNameIndex = "idx_users_username";
            public const string UsersEmailIndex = "idx_users_email";

            public const string TemplatesAuthorIndex = "idx_templates_author";
            public const string TemplatesBaseTemplateIndex = "idx_templates_base_template";
            public const string TemplatesBaseVersionIndex = "idx_templates_base_version";

            public const string LikesTemplateUserIndex = "idx_likes_template_user";
            public const string LikesTemplateIndex = "idx_likes_template";

            public const string TagsNameIndex = "idx_tags_name";

            public const string FormsTemplateIndex = "idx_forms_template";
            public const string FormsUserIndex = "idx_forms_user";
            public const string FormsTemplateUserIndex = "idx_forms_template_user";

            public const string QuestionsTemplateIndex = "idx_questions_template";
            public const string QuestionsTemplateOrderIndex = "idx_questions_template_order";

            public const string CommentsTemplateIndex = "idx_comments_template";

            public const string TemplateTagsTemplateTagIndex = "idx_template_tags_template_tag";
            public const string TemplateTagsTagIndex = "idx_template_tags_tag";

            public const string UserContactsUserIndex = "idx_user_contacts_user";
            public const string UserContactsUserTypePrimaryIndex = "idx_user_contacts_user_type_primary";

            public const string EmailPasswordAuthsEmailIndex = "idx_email_password_auths_email";
            public const string EmailPasswordAuthsUserIndex = "idx_email_password_auths_user";

            public const string GoogleAuthsGoogleIdIndex = "idx_google_auths_google_id";
            public const string GoogleAuthsUserIndex = "idx_google_auths_user";

            public const string TemplateAllowedUsersTemplateUserIndex = "idx_template_allowed_users_template_user";
            public const string TemplateAllowedUsersTemplateIndex = "idx_template_allowed_users_template";
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