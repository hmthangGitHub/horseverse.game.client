using io.hverse.game.protogen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

public class ProtobufMessageParser : IMessageParser
{
    public IMessage Parse(byte[] rawMessage)
    {
        var message = GameMessage.Parser.ParseFrom(rawMessage);
        return message.MsgType switch
        {
            GameMessageType.LoginMessage => message.MsgData.Unpack<LoginRequest>(),
            _ => default
        };
    }

    public byte[] ToByteArray(IMessage message)
    {
        var msg = new GameMessage();
        switch (message.GetType().Name)
        {
            case nameof(LoginRequest):
                msg.MsgType = GameMessageType.LoginMessage;
                msg.MsgData = Any.Pack(message);
                break;
            default:
                break;
        }
        return msg.ToByteArray();
    }
}
