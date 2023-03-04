using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace io.hverse.game.protogen
{
    public sealed partial class RacingMessage : ISubMessage<RacingMessageType>
    {
        Enum ISubMessage.MsgType => this.MsgType;
        public GameMessageType GameMessageType => GameMessageType.RacingMessage;
        public RacingMessage(RaceScriptRequest request)
        {
            this.msgType_ = RacingMessageType.RaceScriptRequest;
            RaceScriptRequest = request;
        }
        
        public RacingMessage(JoinRoomRequest request)
        {
            this.msgType_ = RacingMessageType.JoinRoomRequest;
            JoinRoomRequest = request;
        }

        public RacingMessage(ExitRoomRequest request)
        {
            this.msgType_ = RacingMessageType.ExitRoomRequest;
            ExitRoomRequest = request;
        }

        public RacingMessage(GetRaceHistoryRequest request)
        {
            this.msgType_ = RacingMessageType.GetRaceHistoryRequest;
            GetRaceHistoryRequest = request;
        }

        public RacingMessage(GetRaceDetailRequest request)
        {
            this.msgType_ = RacingMessageType.GetRaceDetailRequest;
            GetRaceDetailRequest = request;
        }

        public RacingMessage(GetRaceReplayRequest request)
        {
            this.msgType_ = RacingMessageType.GetRaceReplayRequest;
            GetRaceReplayRequest = request;
        }

        public RacingMessage(StartRoomRequest request)
        {
            this.msgType_ = RacingMessageType.StartRoomRequest;
            StartRoomRequest = request;
        }
    }
}
