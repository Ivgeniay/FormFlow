using FormFlow.Application.DTOs.Templates;
using FormFlow.Application.DTOs.Users;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Exceptions;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Interfaces.Services;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Services
{
    public class FormSubscribeService : IFormSubscribeService
    {
        private readonly IFormSubscribeRepository _subscribeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IFormRepository _formRepository;
        private readonly IEmailService _emailService;

        public FormSubscribeService(
            IFormSubscribeRepository subscribeRepository,
            IUserRepository userRepository,
            ITemplateRepository templateRepository,
            IFormRepository formRepository,
            IEmailService emailService)
        {
            _subscribeRepository = subscribeRepository;
            _userRepository = userRepository;
            _templateRepository = templateRepository;
            _formRepository = formRepository;
            _emailService = emailService;
        }

        public async Task<bool> SubscribeAsync(Guid userId, Guid templateId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            if (user.IsBlocked)
                throw new UserBlockedException(userId);

            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            if (!await _templateRepository.HasUserAccessAsync(templateId, userId))
                throw new TemplateAccessDeniedException(templateId, userId);

            if (await _subscribeRepository.ExistAsync(userId, templateId))
                return false;

            var subscription = new FormSubscribe
            {
                UserId = userId,
                TemplateId = templateId
            };

            await _subscribeRepository.AddAsync(subscription);
            return true;
        }

        public async Task<bool> UnsubscribeAsync(Guid userId, Guid templateId) =>
            await _subscribeRepository.DeleteAsync(userId, templateId);
        
        public async Task<bool> IsSubscribedAsync(Guid userId, Guid templateId) =>
            await _subscribeRepository.ExistAsync(userId, templateId);
        
        public async Task<List<TemplateListItemDto>> GetUserSubscriptionsAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var templates = await _subscribeRepository.GetByUserIdAsync(userId);

            return templates.Select(template => new TemplateListItemDto
            {
                Id = template.Id,
                Title = template.Title,
                Description = template.Description,
                ImageUrl = template.ImageUrl,
                AuthorName = template.Author?.UserName ?? string.Empty,
                CreatedAt = template.CreatedAt,
                Tags = new List<string>(),
                FormsCount = template.FormsCount,
                LikesCount = template.LikesCount
            }).ToList();
        }

        public async Task<List<UserSearchDto>> GetSubscribersAsync(Guid templateId)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            var users = await _subscribeRepository.GetByTemplateIdAsync(templateId);

            return users.Select(user => new UserSearchDto
            {
                Id = user.Id,
                UserName = user.UserName,
                PrimaryEmail = user.Contacts?.FirstOrDefault(c => c.IsPrimary && c.Type == ContactType.Email)?.Value ?? string.Empty
            }).ToList();
        }

        public async Task NotifySubscribersAsync(Guid formId)
        {
            var form = await _formRepository.GetWithTemplateAsync(formId);
            if (form == null)
                return;

            if (!await _subscribeRepository.HasTemplateSubscribersAsync(form.TemplateId))
                return;

            var subscribers = await _subscribeRepository.GetByTemplateIdAsync(form.TemplateId);

            foreach (var subscriber in subscribers)
            {
                var primaryEmail = subscriber.Contacts?.FirstOrDefault(c => c.IsPrimary && c.Type == ContactType.Email)?.Value;

                if (!string.IsNullOrEmpty(primaryEmail))
                {
                    try
                    {
                        await _emailService.SendFormAnswersAsync(primaryEmail, formId);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
