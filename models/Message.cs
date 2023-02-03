public class Message
{

    public string topic { get; set; } = string.Empty;
    public string args { get; set; } = string.Empty;

    public Message(string topic)
    {
        this.topic = topic;
    }

    public static Message? CreateMessage(MessageType type)
    {
        Message constructedMessage = new Message(MessageTypeEnum.GetTopic(type));
        return constructedMessage;
    }
}