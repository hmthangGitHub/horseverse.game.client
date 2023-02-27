using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.hverse.game.protogen
{
    public sealed partial class CommonMessage : ISubMessage<CommonMessageType>
    {
        Enum ISubMessage.MsgType => this.MsgType;
        public GameMessageType GameMessageType => GameMessageType.CommonMessage;
    }
}
