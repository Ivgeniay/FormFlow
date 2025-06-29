using FormFlow.Application.DTOs.Users;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.DTOs
{
    public static class DTOMapper
    {
        public static UserDto MapToUserDto(User user)
        {
            var dto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Role = user.Role,
                IsBlocked = user.IsBlocked,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                TemplatesCount = user.Templates.Count(),
                FormsCount = user.Forms.Count(),
                Contacts = user.Contacts?.Select(c => new UserContactDto
                {
                    Id = c.Id,
                    Type = c.Type,
                    Value = c.Value,
                    IsPrimary = c.IsPrimary,
                    CreatedAt = c.CreatedAt
                }).ToList() ?? new List<UserContactDto>(),
                AuthMethods = new List<AuthMethodDto>()
            };

            if (user.EmailAuth != null)
            {
                dto.AuthMethods.Add(new EmailAuthMethodDto
                {
                    Type = "Email",
                    IsActive = true,
                    Email = user.EmailAuth.Email
                });
            }

            if (user.GoogleAuth != null)
            {
                dto.AuthMethods.Add(new GoogleAuthMethodDto
                {
                    Type = "Google",
                    IsActive = true,
                    GoogleId = user.GoogleAuth.GoogleId,
                    Email = user.GoogleAuth.Email,
                });
            }

            return dto;
        }
        
        public static UserListItemDto MapToUserListItemDto(User user)
        {
            var primaryEmail = user.Contacts?.FirstOrDefault(c => c.IsPrimary && c.Type == ContactType.Email)?.Value ?? string.Empty;

            return new UserListItemDto
            {
                Id = user.Id,
                UserName = user.UserName,
                PrimaryEmail = primaryEmail,
                Role = user.Role,
                IsBlocked = user.IsBlocked,
                CreatedAt = user.CreatedAt,
                TemplatesCount = 0,
                FormsCount = 0
            };
        }
        
        public static UserSearchDto MapToUserSearchDto(User user)
        {
            var primaryEmail = user.Contacts?.FirstOrDefault(c => c.IsPrimary && c.Type == ContactType.Email)?.Value ?? string.Empty;

            return new UserSearchDto
            {
                Id = user.Id,
                UserName = user.UserName,
                PrimaryEmail = primaryEmail
            };
        }
        
    }
}
