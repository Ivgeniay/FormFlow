namespace FormFlow.Infrastructure
{
    public static class Constants
    {
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
