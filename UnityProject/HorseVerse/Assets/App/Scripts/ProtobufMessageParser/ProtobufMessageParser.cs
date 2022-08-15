using io.hverse.game.protogen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Linq;

public class ProtobufMessageParser : IMessageParser
{
    private readonly Dictionary<System.Enum, Func<ISubMessage, IMessage>> lookUpToMessageFunc = new Dictionary<System.Enum, Func<ISubMessage, IMessage>>();
    private readonly Dictionary<string, Func<Any, ISubMessage>> lookUpToISubMessageFunc = new Dictionary<string, Func<Any, ISubMessage>>();
    private readonly Dictionary<System.Type, Func<IMessage, byte[]>> serializeLookUpFunc = new Dictionary<System.Type, Func<IMessage, byte[]>>(); 
    public ProtobufMessageParser()
    {
        AddToParseLookUpTable<RaceMessage, RaceMessageType>(RaceMessageType.RaceScriptResponse, x => x.RaceScriptResponse);
        AddToSerializeLookUpTable<RaceScriptRequest>(x => new RaceMessage(x));
    }
    
    private void AddToParseLookUpTable<TSubMessage, TEnum>(TEnum enumMessage, Func<TSubMessage, IMessage> resultFactory) where TEnum : System.Enum
                                                                                                                         where TSubMessage : ISubMessage, IMessage, ISubMessage<TEnum>, new()
    {
        TSubMessage message = new TSubMessage();
        lookUpToISubMessageFunc.Add(message.Descriptor.FullName, x => x.Unpack<TSubMessage>());
        lookUpToMessageFunc.Add(enumMessage, x => resultFactory((TSubMessage)x));
    }
    
    private void AddToSerializeLookUpTable<T>(Func<T, ISubMessage> serializeFunc) where T : Google.Protobuf.IMessage, new()
    {
        serializeLookUpFunc.Add(typeof(T), x =>
        {
            var subMessage = serializeFunc((T)x);
            return new GameMessage()
            {
                MsgType = subMessage.gameMessageType,
                MsgData = Any.Pack(subMessage)
            }.ToByteArray();
        });
    }

    public IMessage Parse(byte[] rawMessage)
    {
        var message = GameMessage.Parser.ParseFrom(rawMessage); 
        var iSubMessage = lookUpToISubMessageFunc[Any.GetTypeName(message.MsgData.TypeUrl)](message.MsgData);
        return lookUpToMessageFunc[iSubMessage.MsgType](iSubMessage);
    }

    public byte[] ToByteArray(IMessage message)
    {
        return serializeLookUpFunc[message.GetType()](message);
    }
}