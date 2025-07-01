using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Interfaces
{
    public interface ITopicService
    {
        Task<Topic> CreateTopicAsync(string name);
        Task<Topic?> GetTopicByIdAsync(Guid id);
        Task<PagedResult<Topic>> GetTopicsAsync(int page, int pageSize);
        Task<List<Topic>> GetTopicsAsync(int count);
        Task<Topic> UpdateTopicAsync(Guid id, string name);
        Task DeleteTopicAsync(Guid id);
        Task<bool> TopicExistsAsync(Guid id);
        Task<bool> TopicNameExistsAsync(string name);
        Task<List<Topic>> SearchTopicAsync(string q, int limit);
    }
}
