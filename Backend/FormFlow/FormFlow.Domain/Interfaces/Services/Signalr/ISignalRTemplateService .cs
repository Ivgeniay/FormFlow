using FormFlow.Domain.Models.SignalRModels;

namespace FormFlow.Domain.Interfaces.Services.Signalr
{
    public interface ISignalRTemplateService 
    {
        Task JoinTemplateGroupAsync(Guid templateId, Guid userId);
        Task LeaveTemplateGroupAsync(Guid templateId, Guid userId);

        Task SendCommentToTemplateAsync(Guid templateId, CommentNotification comment);

        Task SendLikeUpdateToTemplateAsync(Guid templateId, LikeUpdateNotification update);
    }

}
