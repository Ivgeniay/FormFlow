using FormFlow.Domain.Interfaces.Repositories;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Question>> GetQuestionsByTemplateAsync(Guid templateId)
        {
            return await _context.Questions
                .Where(q => q.TemplateId == templateId)
                .OrderBy(q => q.Order)
                .ToListAsync();
        }

        public async Task<bool> CreateRangeForTemplateAsync(List<Question> questions)
        {
            try
            {
                if (questions.Any())
                {
                    await _context.Questions.AddRangeAsync(questions);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Question?> GetQuestionByIdAsync(Guid questionId)
        {
            return await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == questionId);
        }

        public async Task<bool> UpdateQuestionsAsync(List<Question> newValue)
        {
            try
            {
                if (newValue.Any())
                {
                    _context.Questions.UpdateRange(newValue);
                    await _context.SaveChangesAsync ();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateQuestionAsync(Question newValue)
        {
            try
            {
                _context.Questions.Update(newValue);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
