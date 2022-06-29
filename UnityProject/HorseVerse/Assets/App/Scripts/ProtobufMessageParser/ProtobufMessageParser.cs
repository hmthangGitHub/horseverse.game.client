using io.hverse.game.protogen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System;

public class ProtobufMessageParser : IMessageParser
{
    public IMessage Parse(byte[] rawMessage)
    {
        var message = GameMessage.Parser.ParseFrom(rawMessage);
        switch (message.MsgType)
        {
            case GameMessageType.LoginMessage:
                {
                    var dataMessage = message.MsgData.Unpack<LoginMessage>();
                    switch (dataMessage.MsgType)
                    {
                        case LoginMessageType.LoginResponse:
                            return dataMessage.LoginResponse;
                        default:
                            throw new UnKnownServerMessageTypeException($"{message.MsgType}.{dataMessage.MsgType}");
                    }
                }
            case GameMessageType.MasterDataMessage:
                {
                    var dataMessage = message.MsgData.Unpack<PlayerMessage>();
                    switch (dataMessage.MsgType)
                    {
                        default:
                            throw new UnKnownServerMessageTypeException($"{message.MsgType}.{dataMessage.MsgType}");
                    }
                }
        }
        return message.MsgType switch
        {
            GameMessageType.LoginMessage => message.MsgData.Unpack<LoginRequest>(),
            GameMessageType.MasterDataMessage => message.MsgData.Unpack<LoginRequest>(),
            _ => default
        };
    }

    public byte[] ToByteArray(IMessage message)
    {
        GameMessageType messageType = default;
        IMessage data = default;

        switch (message.GetType().Name)
        {
            case nameof(LoginRequest):
                {
                    messageType = GameMessageType.LoginMessage;
                    data = new LoginMessage()
                    {
                        MsgType = LoginMessageType.LoginRequest,
                        LoginRequest = (LoginRequest)message
                    };
                    break;
                }
            case nameof(PlayerInventoryRequest):
                {
                    messageType = GameMessageType.PlayerMessage;
                    data = new PlayerMessage()
                    {
                        MsgType = PlayerMessageType.PlayerInventoryRequest,
                        PlayerInventoryRequest = (PlayerInventoryRequest)message
                    };
                    break;
                }
            default:
                break;
        }

        return new GameMessage()
        {
            MsgType = messageType,
            MsgData = Any.Pack(data)
        }.ToByteArray();
    }
}
