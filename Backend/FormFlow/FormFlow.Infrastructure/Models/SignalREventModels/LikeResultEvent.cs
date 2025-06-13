using FormFlow.Application.DTOs.Likes;

namespace FormFlow.Infrastructure.Models.SignalREventModels
{
    public class LikeResultEvent
    {
        public bool Success { get; set; }
        public LikeResultDto Result { get; set; } = new LikeResultDto();
    }
}
