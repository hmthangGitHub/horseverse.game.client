using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace io.hverse.game.protogen
{
    public sealed partial class PlayerMessage : ISubMessage<PlayerMessageType>
    {
        Enum ISubMessage.MsgType => this.MsgType;
        public GameMessageType gameMessageType => GameMessageType.PlayerMessage;

        public PlayerMessage(PlayerInventoryRequest request)
        {
            this.msgType_ = PlayerMessageType.PlayerInventoryRequest;
            playerInventoryRequest_ = request;
        }

    }
}