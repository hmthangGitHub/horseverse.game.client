using Google.Protobuf;

public interface IMessageParser
{
    public IMessage Parse(byte[] rawMessage);
    public byte[] ToByteArray(IMessage message);
}