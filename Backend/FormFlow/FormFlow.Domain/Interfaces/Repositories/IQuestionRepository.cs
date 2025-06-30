using FormFlow.Domain.Models.General;

namespace FormFlow.Domain.Interfaces.Repositories
{
    public interface IQuestionRepository
    {
        public Task<List<Question>> GetQuestionsByTemplateAsync(Guid templateId);
        public Task<bool> CreateRangeForTemplateAsync(List<Question> questions);
        public Task<Question> GetQuestionByIdAsync(Guid questionId);
        public Task<bool> UpdateQuestionsAsync(List<Question> newValue);
        public Task<bool> UpdateQuestionAsync(Question newWalue);
        public Task DeleteQuestionsAsync(List<Guid> questionsToDelete);
        public Task CreateQuestionsAsync(List<Question> newQuestions);
    }

}
