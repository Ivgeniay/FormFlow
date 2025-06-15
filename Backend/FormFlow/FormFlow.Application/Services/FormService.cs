using FormFlow.Application.DTOs.Forms;
using FormFlow.Application.Interfaces;
using FormFlow.Domain.Exceptions;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Interfaces.Services;
using FormFlow.Domain.Models.General;
using FormFlow.Domain.Models.General.QuestionDetailsModels;
using System.Text.Json;

namespace FormFlow.Application.Services
{
    public class FormService : IFormService
    {
        private readonly IFormRepository _formRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFormSubscribeService _formSubscribeService;

        public FormService(
            IFormRepository formRepository,
            ITemplateRepository templateRepository,
            IUserRepository userRepository,
            IFormSubscribeService formSubscribeService)
        {
            _formRepository = formRepository;
            _templateRepository = templateRepository;
            _userRepository = userRepository;
            _formSubscribeService = formSubscribeService;
        }

        public async Task<FormDto> SubmitFormAsync(SubmitFormRequest request, Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            if (user.IsBlocked)
                throw new UserBlockedException(userId);

            var template = await _templateRepository.GetByIdAsync(request.TemplateId);
            if (template == null)
                throw new TemplateNotFoundException(request.TemplateId);
            if (template.IsDeleted)
                throw new TemplateDeletedException(request.TemplateId);
            if (!template.IsPublished)
                throw new TemplateNotPublishedException(request.TemplateId);

            if (!await _templateRepository.HasUserAccessAsync(request.TemplateId, userId))
                throw new TemplateAccessDeniedException(request.TemplateId, userId);

            if (await _formRepository.HasUserSubmittedAsync(request.TemplateId, userId))
                throw new FormAlreadySubmittedException(request.TemplateId, userId);

            var form = new Form
            {
                TemplateId = request.TemplateId,
                UserId = userId,
                AnswersData = JsonSerializer.Serialize(request.Answers),
                SubmittedAt = DateTime.UtcNow,
                TemplateVersion = template.Version,
                IsDeleted = false,
                UpdatedAt = DateTime.UtcNow,
            };

            var createdForm = await _formRepository.CreateAsync(form);
            await _formSubscribeService.NotifySubscribersAsync(createdForm.Id);

            return await MapToFormDtoAsync(createdForm, userId);
        }

        public async Task<FormDto> UpdateFormAsync(UpdateFormRequest request, Guid userId)
        {
            var form = await _formRepository.GetWithAllDetailsAsync(request.Id);
            if (form == null)
                throw new FormNotFoundException(request.Id);

            if (!await CanUserEditFormAsync(request.Id, userId))
                throw new UnauthorizedAccessException("User cannot edit this form");

            var template = await _templateRepository.GetWithQuestionsAsync(form.TemplateId);
            if (template == null)
                throw new TemplateNotFoundException(form.TemplateId);

            form.AnswersData = JsonSerializer.Serialize(request.Answers);
            form.UpdatedAt = DateTime.UtcNow;

            var updatedForm = await _formRepository.UpdateAsync(form);
            return await MapToFormDtoAsync(updatedForm, userId);
        }

        public async Task DeleteFormAsync(Guid formId, Guid userId)
        {
            var form = await _formRepository.GetByIdAsync(formId);
            if (form == null)
                throw new FormNotFoundException(formId);

            if (!await CanUserEditFormAsync(formId, userId))
                throw new UnauthorizedAccessException("User cannot delete this form");

            await _formRepository.DeleteAsync(formId);
        }

        public async Task<FormDto> GetFormByIdAsync(Guid formId, Guid userId)
        {
            var form = await _formRepository.GetWithAllDetailsAsync(formId);
            if (form == null)
                throw new FormNotFoundException(formId);

            if (!await CanUserViewFormAsync(formId, userId))
                throw new UnauthorizedAccessException("User cannot view this form");

            return await MapToFormDtoAsync(form, userId);
        }

        public async Task<bool> FormExistsAsync(Guid formId)
        {
            return await _formRepository.ExistsAsync(formId);
        }

        public async Task<PagedResult<FormDto>> GetFormsByTemplatePagedAsync(Guid templateId, int page, int pageSize, Guid userId)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
                throw new TemplateNotFoundException(templateId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            bool canViewAllForms = user.Role.HasFlag(UserRole.Admin) ||
                                  await _templateRepository.IsAuthorAsync(templateId, userId);

            if (!canViewAllForms)
                throw new UnauthorizedAccessException("User cannot view forms for this template");

            var result = await _formRepository.GetFormsByTemplatePagedAsync(templateId, page, pageSize);
            var formDtos = new List<FormDto>();

            foreach (var form in result.Data)
                formDtos.Add(await MapToFormDtoAsync(form, userId));


            return new PagedResult<FormDto>(formDtos, result.Pagination.TotalCount, page, pageSize);
        }

        public async Task<PagedResult<FormDto>> GetFormsByUserPagedAsync(Guid userId, int page, int pageSize)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var result = await _formRepository.GetFormsByUserPagedAsync(userId, page, pageSize);
            var formDtos = new List<FormDto>();

            foreach (var form in result.Data)
            {
                formDtos.Add(await MapToFormDtoAsync(form, userId));
            }

            return new PagedResult<FormDto>(formDtos, result.Pagination.TotalCount, page, pageSize);
        }


        public async Task<List<FormDto>> GetUserFormsForAllVersionsAsync(Guid baseTemplateId, Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            if (!await _templateRepository.BaseTemplateExistsAsync(baseTemplateId))
                throw new TemplateNotFoundException(baseTemplateId);

            var forms = await _formRepository.GetUserFormsForAllVersionsAsync(baseTemplateId, userId);
            var formDtos = new List<FormDto>();

            foreach (var form in forms)
            {
                formDtos.Add(await MapToFormDtoAsync(form, userId));
            }

            return formDtos;
        }

        public async Task<FormDto?> GetUserFormForTemplateAsync(Guid templateId, Guid userId)
        {
            var form = await _formRepository.GetUserFormForTemplateAsync(templateId, userId);
            if (form == null)
                return null;

            return await MapToFormDtoAsync(form, userId);
        }

        public async Task<bool> HasUserSubmittedFormAsync(Guid templateId, Guid userId)
        {
            return await _formRepository.HasUserSubmittedAsync(templateId, userId);
        }

        public async Task<bool> CanUserViewFormAsync(Guid formId, Guid userId)
        {
            var form = await _formRepository.GetWithAllDetailsAsync(formId);
            if (form == null) return false;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (user.Role.HasFlag(UserRole.Admin)) return true;
            if (form.UserId == userId) return true;

            return await _templateRepository.IsAuthorAsync(form.TemplateId, userId);
        }

        public async Task<bool> CanUserEditFormAsync(Guid formId, Guid userId)
        {
            var form = await _formRepository.GetByIdAsync(formId);
            if (form == null) return false;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (user.Role.HasFlag(UserRole.Admin)) return true;
            if (form.UserId == userId) return true;

            return await _templateRepository.IsAuthorAsync(form.TemplateId, userId);
        }

        
        private async Task<FormDto> MapToFormDtoAsync(Form form, Guid userId)
        {
            var template = await _templateRepository.GetWithQuestionsAsync(form.TemplateId);
            var answers = JsonSerializer.Deserialize<Dictionary<Guid, object>>(form.AnswersData) ?? new Dictionary<Guid, object>();

            var dto = new FormDto
            {
                Id = form.Id,
                TemplateId = form.TemplateId,
                TemplateName = template?.Title ?? "Unknown Template",
                UserId = form.UserId,
                UserName = form.User?.UserName ?? "Unknown User",
                SubmittedAt = form.SubmittedAt,
                UpdatedAt = form.UpdatedAt,
                Questions = new List<FormQuestionDto>(),
                CanEdit = await CanUserEditFormAsync(form.Id, userId),
                CanDelete = await CanUserEditFormAsync(form.Id, userId)
            };

            if (template?.Questions != null)
            {
                foreach (var question in template.Questions.Where(q => !q.IsDeleted).OrderBy(q => q.Order))
                {
                    var questionDetails = JsonSerializer.Deserialize<QuestionDetails>(question.Data);
                    var answerValue = answers.ContainsKey(question.Id) ? answers[question.Id] : null;

                    dto.Questions.Add(new FormQuestionDto
                    {
                        QuestionId = question.Id,
                        Title = questionDetails?.Title ?? "Unknown Question",
                        Description = questionDetails?.Description ?? string.Empty,
                        Type = questionDetails?.Type ?? QuestionType.ShortText,
                        Order = question.Order,
                        IsRequired = question.IsRequired,
                        QuestionData = question.Data,
                        Answer = answerValue
                    });
                }
            }

            return dto;
        }

    }
}
