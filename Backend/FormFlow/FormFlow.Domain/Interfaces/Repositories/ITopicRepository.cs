using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface ITopicRepository
    {
        Task<Topic> GetTopicByIdAsync(Guid topicId);
        Task<PagedResult<Topic>> GetTopicsAsync(int count, int page);
        Task<bool> AddTopicAsync(string topic);
        Task<bool> AddTopicsAsync(List<string> topics);
    }
}
