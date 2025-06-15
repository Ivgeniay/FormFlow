using FormFlow.Application.Interfaces;
using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;

namespace FormFlow.Application.Services
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _topicRepository;

        public TopicService(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public async Task<Topic> CreateTopicAsync(string name)
        {
            if (await _topicRepository.TopicNameExistsAsync(name))
                throw new ArgumentException($"Topic with name '{name}' already exists");

            var topic = new Topic
            {
                Name = name.Trim()
            };

            return await _topicRepository.CreateTopicAsync(topic);
        }

        public async Task<Topic?> GetTopicByIdAsync(Guid id)
        {
            return await _topicRepository.GetTopicByIdAsync(id);
        }

        public async Task<PagedResult<Topic>> GetTopicsAsync(int page, int pageSize)
        {
            return await _topicRepository.GetTopicsAsync(pageSize, page);
        }

        public async Task<List<Topic>> GetTopicsAsync(int count)
        {
            var result = await _topicRepository.GetTopicsAsync(count, 1);
            return result.Data;
        }

        public async Task<Topic> UpdateTopicAsync(Guid id, string name)
        {
            var topic = await _topicRepository.GetTopicByIdAsync(id);
            if (topic == null)
                throw new ArgumentException($"Topic with ID '{id}' not found");

            if (await _topicRepository.TopicNameExistsAsync(name) && topic.Name != name.Trim())
                throw new ArgumentException($"Topic with name '{name}' already exists");

            return await _topicRepository.UpdateTopicAsync(id, name.Trim());
        }

        public async Task DeleteTopicAsync(Guid id)
        {
            var topic = await _topicRepository.GetTopicByIdAsync(id);
            if (topic == null)
                throw new ArgumentException($"Topic with ID '{id}' not found");

            await _topicRepository.DeleteTopicAsync(id);
        }

        public async Task<bool> TopicExistsAsync(Guid id)
        {
            var topic = await _topicRepository.GetTopicByIdAsync(id);
            return topic != null;
        }

        public async Task<bool> TopicNameExistsAsync(string name)
        {
            return await _topicRepository.TopicNameExistsAsync(name.Trim());
        }
    }
}
