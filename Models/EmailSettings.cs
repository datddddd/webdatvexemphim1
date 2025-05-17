namespace ck.Models
{
    public class EmailSettings
    {
        public string FromEmail { get; set; } = string.Empty;
        public string AppPassword { get; set; } = string.Empty;
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}
