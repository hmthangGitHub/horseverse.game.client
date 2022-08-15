using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace io.hverse.game.protogen
{
    public sealed partial class RaceMessage : ISubMessage<RaceMessageType>
    {
        Enum ISubMessage.MsgType => this.MsgType;
        public GameMessageType gameMessageType => GameMessageType.RaceMessage;
        public RaceMessage(RaceScriptRequest request)
        {
            this.msgType_ = RaceMessageType.RaceScriptRequest;
            raceScriptRequest_ = request;
        }
    }
}
