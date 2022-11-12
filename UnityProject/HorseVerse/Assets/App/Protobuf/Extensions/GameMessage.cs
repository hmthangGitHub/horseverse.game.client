using System.Collections;
using System.Collections.Generic;
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
    }
}