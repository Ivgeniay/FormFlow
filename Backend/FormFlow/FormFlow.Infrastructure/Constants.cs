namespace FormFlow.Infrastructure
{
    public static class Constants
    {
        public static class SignalREvents
        {
            public const string NEW_COMMENT = "NewComment";
            public const string LIKE_TOGGLED = "LikeToggled";
            public const string USER_JOINED = "UserJoined";
            public const string USER_LEFT = "UserLeft";
            public const string ERROR = "Error";
            public const string COMMENT_ADDED = "CommentAdded";
            public const string USER_DISCONNECTED = "UserDisconnected";
            public const string TEMPLATE_ACTIVITY = "TemplateActivity";
            public const string LIKE_RESULT = "LikeResult";
            public const string JOINED_TEMPLATE = "JoinedTemplate";
            public const string CONNECTED = "Connected";
        }
        public static class Auth
        {
            public const string CURRENT_USER_KEY = "CurrentUser";
        }

        public static class JwtClaimNames
        {
            public const string Subject = "sub";
            public const string Name = "name";
            public const string Email = "email";
            public const string Role = "role";
            public const string AuthType = "auth_type";
            public const string IsBlocked = "is_blocked";
            public const string IssuedAt = "issued_at";
            public const string TokenId = "token_id";
        }
    }
}
