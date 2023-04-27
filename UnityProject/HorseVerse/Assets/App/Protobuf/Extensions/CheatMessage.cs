using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.hverse.game.protogen
{
    public partial class CheatMessage : ISubMessage<CheatMessageType>
    {
        public GameMessageType GameMessageType => GameMessageType.CheatMessage;
        Enum ISubMessage.MsgType => msgType_;
        
        public CheatMessage(CheatHorseInfoRequest x)
        {
            MsgType = CheatMessageType.CheatHorseInfoRequest;
            CheatHorseInfoRequest = x;
        }

        public CheatMessage(CheatPlayerInfoRequest x)
        {
            MsgType = CheatMessageType.CheatPlayerInfoRequest;
            CheatPlayerInfoRequest = x;
        }
    }
}