using System;
using Google.Protobuf;

namespace io.hverse.game.protogen
{
    public partial class DataMessage : ISubMessage<DataMessageType>
    {
        Enum ISubMessage.MsgType => this.MsgType;
        public GameMessageType GameMessageType => GameMessageType.DataMessage;
        public DataMessage(MasterDataRequest x)
        {
            MsgType = DataMessageType.MasterDataRequest;
            MasterDataRequest = x;
        }
    }
}
