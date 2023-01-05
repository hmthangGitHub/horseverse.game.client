using System;
using Google.Protobuf;

namespace io.hverse.game.protogen
{
    public partial class TrainingMessage : ISubMessage<TrainingMessageType>
    {
        Enum ISubMessage.MsgType => this.MsgType;
        public GameMessageType GameMessageType => GameMessageType.TrainingMessage;
        public TrainingMessage(StartTrainingRequest x)
        {
            MsgType = TrainingMessageType.StartTrainingRequest;
            StartTrainingRequest = x;
        }

        public TrainingMessage(FinishTrainingRequest x)
        {
            MsgType = TrainingMessageType.FinishTrainingRequest;
            FinishTrainingRequest = x;
        }
    }
}
