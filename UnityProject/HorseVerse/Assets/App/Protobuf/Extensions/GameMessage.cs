using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Google.Protobuf;
namespace io.hverse.game.protogen
{
    public partial class GameMessage : IMessage
    {
        public byte[] ToByteArray()
        {
            var messageSerializeSize = this.CalculateSize();
            var messageSizeAdditionalHeader = CodedOutputStream.ComputeRawVarint32Size((uint)(messageSerializeSize));
            var totalSize = messageSizeAdditionalHeader + messageSerializeSize;
            var data = new byte[totalSize];
            var co = new CodedOutputStream(data);
            co.WriteUInt32((uint)(messageSerializeSize));
            WriteTo(co);
            return data;
        }

        public static GameMessage ParseFromRawMessageWithAdditionalSizeHeader(byte[] rawMessage)
        {
            return Parser.ParseFrom(GetDataFromRawMessageWithSizeAppendAhead(rawMessage));
        }
        
        private static byte[] GetDataFromRawMessageWithSizeAppendAhead(byte[] rawMessage)
        {
            var messageSizeByteArray = rawMessage.TakeWhile(x => (x & 0x80) != 0)
                .Append(rawMessage.FirstOrDefault(x => (x & 0x80) == 0))
                .ToArray();
            var codedInputStream = new CodedInputStream(messageSizeByteArray);
            return rawMessage.Skip(messageSizeByteArray.Length).Take((int)codedInputStream.ReadUInt32()).ToArray();
        }
    }
}
