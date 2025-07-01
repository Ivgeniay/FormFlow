using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface ITopicRepository
    {
        Task<Topic> GetTopicByIdAsync(Guid topicId);
        Task<PagedResult<Topic>> GetTopicsAsync(int count, int page);
        Task<bool> AddTopicAsync(string topic);
        Task<bool> AddTopicsAsync(List<string> topics);
        Task<bool> TopicNameExistsAsync(string name);
        Task<Topic> CreateTopicAsync(Topic topic);
        Task<Topic> UpdateTopicAsync(Guid id, string v);
        Task DeleteTopicAsync(Guid id);
        Task<List<Topic>> SearchByNameAsync(string normalizedQuery, int limit);
    }
}
