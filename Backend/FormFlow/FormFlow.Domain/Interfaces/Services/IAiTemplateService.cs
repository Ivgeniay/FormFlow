namespace FormFlow.Domain.Interfaces.Services
{
    public interface IAiTemplateService
    {
        Task<string> GenerateFromPromptAsync(string prompt);
    }
}
