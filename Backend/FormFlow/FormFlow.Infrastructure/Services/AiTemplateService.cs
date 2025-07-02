using FormFlow.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace FormFlow.Infrastructure.Services
{
    public class AiTemplateService : IAiTemplateService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly int _timeoutMs;

        public AiTemplateService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["AiService:BaseUrl"];
            _timeoutMs = configuration.GetValue<int>("AiService:TimeoutMs");

            _httpClient.Timeout = TimeSpan.FromMilliseconds(_timeoutMs);
        }

        public async Task<string> GenerateFromPromptAsync(string prompt)
        {
            var request = new { prompt = prompt };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/generate", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"AI service returned {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
