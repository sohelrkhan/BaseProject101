namespace SadaqaAccounting.Shared.Models
{
    public class EmailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public ICollection<IFormFile>? Attachments { get; set; }
    }
}