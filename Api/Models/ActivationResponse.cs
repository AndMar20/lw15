namespace Api.Models
{
    public class ActivationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? LicenseKey { get; set; }
        public DateTime? ActivatedAt { get; set; }
    }
}
