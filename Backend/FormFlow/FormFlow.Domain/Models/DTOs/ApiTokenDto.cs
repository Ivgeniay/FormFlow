namespace FormFlow.Domain.Models.DTOs
{
    public class ApiTokenDto
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}