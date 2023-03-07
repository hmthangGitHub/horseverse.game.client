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
        public GameMessageType GameMessageType => GameMessageType.PlayerMessage;

        public PlayerMessage(PlayerInventoryRequest request)
        {
            this.msgType_ = PlayerMessageType.PlayerInventoryRequest;
            playerInventoryRequest_ = request;
        }

        public PlayerMessage(PlayerInfoRequest request)
        {
            this.msgType_ = PlayerMessageType.PlayerInfoRequest;
            playerInfoRequest_ = request;
        }

    }
}