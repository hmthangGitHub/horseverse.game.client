using System;
using Google.Protobuf;

namespace io.hverse.game.protogen
{
    public partial class TrainingMessage : ISubMessage<TrainingMessageType>
    {
        Enum ISubMessage.MsgType => this.MsgType;
        public GameMessageType GameMessageType => GameMessageType.TrainingMessage;
        public TrainingMessage(TrainingRewardsRequest x)
        {
            MsgType = TrainingMessageType.TrainingRewardsRequest;
            TrainingRewardsRequest = x;
        }
    }
}
