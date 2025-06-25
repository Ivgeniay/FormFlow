using FormFlow.Application.DTOs.Templates;

namespace FormFlow.Application.DTOs.Forms
{
    public class FormAccessDto
    {
        public bool CanFillForm { get; set; }
        public bool HasAlreadySubmitted { get; set; }
        public string? DenialReason { get; set; }
        public FormDto? ExistingForm { get; set; }
    }

   
}
