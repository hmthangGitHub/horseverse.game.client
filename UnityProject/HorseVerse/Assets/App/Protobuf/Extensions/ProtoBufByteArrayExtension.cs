using System.Linq;
using Google.Protobuf;

public static class ProtoBufByteArrayExtension
{
    public static byte[] GetDataFromRawMessageWithSizeAppendAhead(this byte[] rawMessage)
    {
        var messageSizeByteArray = rawMessage.TakeWhile(x => (x & 0x80) != 0)
            .Append(rawMessage.FirstOrDefault(x => (x & 0x80) == 0))
            .ToArray();
        var codedInputStream = new CodedInputStream(messageSizeByteArray);
        return rawMessage.Skip(messageSizeByteArray.Length).Take((int)codedInputStream.ReadUInt32()).ToArray();
    }
}