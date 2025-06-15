using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

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
    }
}
