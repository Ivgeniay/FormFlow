namespace FormFlow.Application.DTOs.Forms
{
    public class UpdateFormRequest
    {
        public Guid Id { get; set; }
        public Dictionary<Guid, object> Answers { get; set; } = new();
    }
}
