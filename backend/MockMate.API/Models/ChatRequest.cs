namespace MockMate.API.Models
{
    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public List<MessageHistory> History { get; set; } = new();

    }

    public class MessageHistory
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}