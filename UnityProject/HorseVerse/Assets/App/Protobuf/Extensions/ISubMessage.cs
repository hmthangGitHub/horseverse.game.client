using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using io.hverse.game.protogen;
using UnityEngine;

public interface ISubMessage : IMessage
{
    System.Enum MsgType { get; }
    GameMessageType GameMessageType { get; }
}

public interface ISubMessage<out T> : ISubMessage where T : System.Enum
{
}