using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.hverse.game.protogen
{
    public partial class BreedingMessage : ISubMessage<BreedingMessageType>
    {
        Enum ISubMessage.MsgType => msgType_;
        public GameMessageType GameMessageType => GameMessageType.BreedingMessage;
        public BreedingMessage(BreedingInfoRequest x)
        {
            MsgType = BreedingMessageType.BreedingInfoRequest;
            BreedingInfoRequest = x;
        }

        public BreedingMessage(BreedingRequest x)
        {
            MsgType = BreedingMessageType.BreedingRequest;
            BreedingRequest = x;
        }

        public BreedingMessage(FinishBreedingRequest x)
        {
            MsgType = BreedingMessageType.FinishBreedingRequest;
            FinishBreedingRequest = x;
        }
    }
}