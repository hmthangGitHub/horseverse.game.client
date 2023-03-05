using System.IO;
using Cysharp.Threading.Tasks;
using Google.Protobuf;

public interface IMessageParser
{
    IMessage Parse(byte[] rawMessage);
    byte[] ToByteArray(IMessage message);
    UniTask<(int size, byte[] sizeByteArray)> GetNextMessageSize(Stream stream);
}