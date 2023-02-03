public enum MessageType
{
    ACK,
    POP,
    SET_IMAGE,
    QUERY
}

public static class MessageTypeEnum
{
    public static string GetTopic(MessageType type)
    {
        string returnType = string.Empty;
        switch (type)
        {
            case MessageType.ACK:
                returnType = "ack";
                break;
            case MessageType.POP:
                returnType = "pop";
                break;
            case MessageType.SET_IMAGE:
                returnType = "set_image";
                break;
            case MessageType.QUERY:
                returnType = "query";
                break;
        }
        return returnType;
    }
}