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
            raceScriptRequest_ = request;
        }
        
        public RacingMessage(JoinRoomRequest request)
        {
            this.msgType_ = RacingMessageType.JoinRoomRequest;
            joinRoomRequest_ = request;
        }

        public RacingMessage(ExitRoomRequest request)
        {
            this.msgType_ = RacingMessageType.ExitRoomRequest;
            exitRoomRequest_ = request;
        }

        public RacingMessage(GetHistoryRequest request)
        {
            this.msgType_ = RacingMessageType.GetHistoryRequest;
            getHistoryRequest_ = request;
        }
    }
}
