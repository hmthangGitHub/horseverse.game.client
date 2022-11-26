using System;
using Google.Protobuf;

namespace io.hverse.game.protogen
{
    public partial class BettingMessage : ISubMessage<BettingMessageType>
    {
        Enum ISubMessage.MsgType => this.MsgType;
        public GameMessageType GameMessageType => GameMessageType.BettingMessage;
        public BettingMessage(GetInfoBettingRequest x)
        {
            MsgType = BettingMessageType.GetInfoBettingRequest;
            GetInfoBettingRequest = x;
        }

        public BettingMessage(GetTheMatchRequest x)
        {
            MsgType = BettingMessageType.GetTheMatchRequest;
            GetTheMatchRequest = x;
        }

        public BettingMessage(SendBettingInfoRequest x)
        {
            MsgType = BettingMessageType.SendBettingInfoRequest;
            SendBettingInfoRequest = x;
        }

        public BettingMessage(CancelBettingRequest x)
        {
            MsgType = BettingMessageType.CancelBettingRequest;
            CancelBettingRequest = x;
        }
        
        public BettingMessage(GetBettingMatchDetailRequest x)
        {
            MsgType = BettingMessageType.GetBettingMatchDetailRequest;
            GetBettingMatchDetailRequest = x;
        }
        
        public BettingMessage(GetBettingMatchRequest x)
        {
            MsgType = BettingMessageType.GetBettingMatchRequest;
            GetBettingMatchRequest = x;
        }
    }    
}

