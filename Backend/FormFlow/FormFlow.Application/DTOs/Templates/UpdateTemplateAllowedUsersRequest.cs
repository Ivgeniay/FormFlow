namespace FormFlow.Application.DTOs.Templates
{
    public class UpdateTemplateAllowedUsersRequest
    {
        public List<Guid> AllowedUserIds { get; set; } = new List<Guid>();
    }
}
