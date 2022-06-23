public interface IMessageParser
{
    public IMessage Parse(byte[] rawMessage);
    public byte[] ToByteArray(IMessage message);
}

public class ProtobufMessageParser : IMessageParser
{
    public IMessage Parse(byte[] rawMessage)
    {
        return default;
    }

    public byte[] ToByteArray(IMessage message)
    {
        return message.ToByteArray();
    }
}