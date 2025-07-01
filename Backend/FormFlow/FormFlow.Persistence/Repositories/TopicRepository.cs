using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FormFlow.Persistence.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly ApplicationDbContext _context;

        public TopicRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<bool> AddTopicAsync(string topic)
        {
            try
            {
                _context.Topics.Add(new Topic
                {
                    Id = Guid.NewGuid(),
                    Name = topic,
                });
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AddTopicsAsync(List<string> topics)
        {
            try
            {
                _context.AddRange(topics.Select(t => new Topic { Name = t }));
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Topic?> GetTopicByIdAsync(Guid topicId) =>
            await _context.Topics
                .FirstOrDefaultAsync(t => t.Id == topicId);


        public async Task<PagedResult<Topic>> GetTopicsAsync(int count, int page)
        {
            var li = await _context.Topics
                .Skip((page - 1) * count)
                .Take(count)
                .ToListAsync();

            var totalCount = await _context.Topics.CountAsync();
            return new PagedResult<Topic>(li, totalCount, page, count);
        }

        public async Task<bool> TopicNameExistsAsync(string name) =>
            await _context.Topics.AnyAsync(t => t.Name == name);

        public async Task<Topic> CreateTopicAsync(Topic topic)
        {
            if (await TopicNameExistsAsync(topic.Name))
                throw new ArgumentException($"Topic with name '{topic.Name}' already exists");
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();
            return topic;
        }

        public async Task<Topic> UpdateTopicAsync(Guid id, string name)
        {
            var topic = await GetTopicByIdAsync(id);
            if (topic == null)
                throw new ArgumentException($"Topic with ID '{id}' not found");
            topic.Name = name.Trim();
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();
            return topic;
        }

        public async Task DeleteTopicAsync(Guid id)
        {
            var topic = await GetTopicByIdAsync(id);
            if (topic == null)
                throw new ArgumentException($"Topic with ID '{id}' not found");
            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Topic>> SearchByNameAsync(string query, int limit)
        {
            var normalizedQuery = query.Trim().ToLower();
            return await _context.Topics
                .Where(t => t.Name.Contains(normalizedQuery))
                .OrderBy(t => t.Name)
                .Take(limit)
                .ToListAsync();
        }
    }
}
