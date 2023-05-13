namespace Dhicoin.Utility
{
    public class Message
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public Message(string toAddress, string subject, string body)
        {
            To = toAddress;
            Subject = subject;
            Content = body;
        }
    }
}
