using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.hverse.game.protogen
{
    public partial class HorseMessage : ISubMessage<HorseMessageType>
    {
        Enum ISubMessage.MsgType => this.MsgType;
        public GameMessageType GameMessageType => GameMessageType.HorseMessage;
        
        public HorseMessage(PlayerHorseBasicRequest x)
        {
            MsgType = HorseMessageType.PlayerHorseBasicRequest;
            PlayerHorseBasicRequest = x;
        }

        public HorseMessage(PlayerHorseAttributeRequest x)
        {
            MsgType = HorseMessageType.PlayerHorseAttributeRequest;
            PlayerHorseAttributeRequest = x;
        }

        public HorseMessage(PlayerHorseRisingRequest x)
        {
            MsgType = HorseMessageType.PlayerHorseRisingRequest;
            PlayerHorseRisingRequest = x;
        }

        public HorseMessage(PlayerHorseHistoryRequest x)
        {
            MsgType = HorseMessageType.PlayerHorseHistoryRequest;
            PlayerHorseHistoryRequest = x;
        }
    }
}