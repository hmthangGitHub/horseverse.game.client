using System;
using Google.Protobuf;

namespace io.hverse.game.protogen
{
    public partial class BettingMessage : ISubMessage<BettingMessageType>
    {
        Enum ISubMessage.MsgType => this.MsgType;
        public GameMessageType GameMessageType => GameMessageType.BettingMessage;
        public BettingMessage(GetCurrentBetMatchRequest x)
        {
            MsgType = BettingMessageType.GetCurrentBetMatchRequest;
            GetCurrentBetMatchRequest = x;
        }

        public BettingMessage(GetCurrentRaceScriptRequest x)
        {
            MsgType = BettingMessageType.GetCurrentRaceScriptRequest;
            GetCurrentRaceScriptRequest = x;
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
        
        public BettingMessage(GetBetHistoryDetailRequest x)
        {
            MsgType = BettingMessageType.GetBettingHistoryDetailRequest;
            GetBetHistoryDetailRequest = x;
        }
        
        public BettingMessage(GetBetHistoryRequest x)
        {
            MsgType = BettingMessageType.GetBetHistoryRequest;
            GetBetHistoryRequest = x;
        }

        public BettingMessage(GetHorseListRequest x)
        {
            MsgType = BettingMessageType.GetHorseListRequest;
            GetHorseListRequest = x;
        }
    }    
}

