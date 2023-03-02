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
        
        public BettingMessage(BetHistoryDetailRequest x)
        {
            MsgType = BettingMessageType.BetHistoryDetailRequest;
            BetHistoryDetailRequest = x;
        }
        
        public BettingMessage(BetHistoryRequest x)
        {
            MsgType = BettingMessageType.BetHistoryRequest;
            BetHistoryRequest = x;
        }

        public BettingMessage(BetHistoryHorseInfoRequest x)
        {
            MsgType = BettingMessageType.BetHistoryHorseInfoRequest;
            BetHistoryHorseInfoRequest = x;
        }

        public BettingMessage(BetHorseListRequest x)
        {
            MsgType = BettingMessageType.BetHorseListRequest;
            BetHorseListRequest = x;
        }
    }    
}

