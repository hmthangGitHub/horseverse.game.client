using io.hverse.game.protogen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System;

public class ProtobufMessageParser : IMessageParser
{
    private readonly Dictionary<System.Enum, Func<ISubMessage, IMessage>> lookUpToMessageFunc = new Dictionary<System.Enum, Func<ISubMessage, IMessage>>();
    private readonly Dictionary<string, Func<Any, ISubMessage>> lookUpToISubMessageFunc = new Dictionary<string, Func<Any, ISubMessage>>();
    private readonly Dictionary<System.Type, Func<IMessage, byte[]>> serializeLookUpFunc = new Dictionary<System.Type, Func<IMessage, byte[]>>(); 
    public ProtobufMessageParser()
    {
        AddToSerializeLookUpTable<RaceScriptRequest>(x => new RacingMessage(x));
        AddToParseLookUpTable<RacingMessage, RacingMessageType>(RacingMessageType.RaceScriptResponse, x => x.RaceScriptResponse);
        
        AddToSerializeLookUpTable<LoginRequest>(x => new LoginMessage(x));
        AddToParseLookUpTable<LoginMessage, LoginMessageType>(LoginMessageType.LoginResponse, x => x.LoginResponse);

        AddToSerializeLookUpTable<EmailCodeRequest>(x => new LoginMessage(x));
        AddToParseLookUpTable<LoginMessage, LoginMessageType>(LoginMessageType.EmailCodeResponse, x => x.EmailCodeResponse);

        AddToSerializeLookUpTable<MasterDataRequest>(x => new DataMessage(x));
        AddToParseLookUpTable<DataMessage, DataMessageType>(DataMessageType.MasterDataResponse, x => x.MasterDataResponse);

        AddToSerializeLookUpTable<PlayerInventoryRequest>(x => new PlayerMessage(x));
        AddToParseLookUpTable<PlayerMessage, PlayerMessageType>(PlayerMessageType.PlayerInventoryResponse, x => x.PlayerInventoryResponse);
        
        AddToSerializeLookUpTable<GetCurrentBetMatchRequest>(x => new BettingMessage(x));
        AddToParseLookUpTable<BettingMessage, BettingMessageType>(BettingMessageType.GetCurrentBetMatchResponse, x => x.GetCurrentBetMatchResponse);
        
        AddToSerializeLookUpTable<GetCurrentRaceScriptRequest>(x => new BettingMessage(x));
        AddToParseLookUpTable<BettingMessage, BettingMessageType>(BettingMessageType.GetCurrentRaceScriptResponse, x => x.GetCurrentRaceScriptResponse);
        
        AddToSerializeLookUpTable<SendBettingInfoRequest>(x => new BettingMessage(x));
        AddToParseLookUpTable<BettingMessage, BettingMessageType>(BettingMessageType.SendBettingInfoResponse, x => x.SendBettingInfoResponse);
        
        AddToSerializeLookUpTable<BetHorseListRequest>(x => new BettingMessage(x));
        AddToParseLookUpTable<BettingMessage, BettingMessageType>(BettingMessageType.BetHorseListResponse, x => x.BetHorseListResponse);
        
        AddToSerializeLookUpTable<CancelBettingRequest>(x => new BettingMessage(x));
        AddToParseLookUpTable<BettingMessage, BettingMessageType>(BettingMessageType.CancelBettingResponse, x => x.CancelBettingResponse);
        
        AddToSerializeLookUpTable<BetHistoryDetailRequest>(x => new BettingMessage(x));
        AddToParseLookUpTable<BettingMessage, BettingMessageType>(BettingMessageType.BetHistoryDetailResponse, x => x.BetHistoryDetailResponse);
        
        AddToSerializeLookUpTable<BetHistoryRequest>(x => new BettingMessage(x));
        AddToParseLookUpTable<BettingMessage, BettingMessageType>(BettingMessageType.BetHistoryResponse, x => x.BetHistoryResponse);

        AddToSerializeLookUpTable<BetHistoryHorseInfoRequest>(x => new BettingMessage(x));
        AddToParseLookUpTable<BettingMessage, BettingMessageType>(BettingMessageType.BetHistoryHorseInfoResponse, x => x.BetHistoryHorseInfoResponse);

        AddToSerializeLookUpTable<StartTrainingRequest>(x => new TrainingMessage(x));
        AddToParseLookUpTable<TrainingMessage, TrainingMessageType>(TrainingMessageType.StartTrainingResponse, x => x.StartTrainingResponse);

        AddToSerializeLookUpTable<FinishTrainingRequest>(x => new TrainingMessage(x));
        AddToParseLookUpTable<TrainingMessage, TrainingMessageType>(TrainingMessageType.FinishTrainingResponse, x => x.FinishTrainingResponse);
        
        AddToSerializeLookUpTable<JoinRoomRequest>(x => new RacingMessage(x));
        AddToParseLookUpTable<RacingMessage, RacingMessageType>(RacingMessageType.JoinRoomResponse, x => x.JoinRoomResponse);
        
        AddToSerializeLookUpTable<ExitRoomRequest>(x => new RacingMessage(x));
        AddToParseLookUpTable<RacingMessage, RacingMessageType>(RacingMessageType.ExitRoomResponse, x => x.ExitRoomResponse);
        
        AddToParseLookUpTable<RacingMessage, RacingMessageType>(RacingMessageType.StartRoomReceipt, x => x.StartRoomReceipt);
        AddToParseLookUpTable<RacingMessage, RacingMessageType>(RacingMessageType.UpdateRoomReceipt, x => x.UpdateRoomReceipt);
        
        AddToSerializeLookUpTable<GetRaceHistoryRequest>(x => new RacingMessage(x));
        AddToParseLookUpTable<RacingMessage, RacingMessageType>(RacingMessageType.GetRaceHistoryResponse, x => x.GetRaceHistoryResponse);
        
        AddToSerializeLookUpTable<GetRaceDetailRequest>(x => new RacingMessage(x));
        AddToParseLookUpTable<RacingMessage, RacingMessageType>(RacingMessageType.GetRaceDetailResponse, x => x.GetRaceDetailResponse);
        
        AddToSerializeLookUpTable<GetRaceReplayRequest>(x => new RacingMessage(x));
        AddToParseLookUpTable<RacingMessage, RacingMessageType>(RacingMessageType.GetRaceReplayResponse, x => x.GetRaceReplayResponse);
        
        AddToParseLookUpTable<CommonMessage, CommonMessageType>(CommonMessageType.RestartGamePopUpMessage, x => x.RestartGamePopUpMessage);
        AddToParseLookUpTable<CommonMessage, CommonMessageType>(CommonMessageType.BroadcastMessage, x => x.BroadcastMessage);
    }
    
    private void AddToParseLookUpTable<TSubMessage, TEnum>(TEnum enumMessage, Func<TSubMessage, IMessage> resultFactory) where TEnum : System.Enum
                                                                                                                         where TSubMessage : ISubMessage, IMessage, ISubMessage<TEnum>, new()
    {
        TSubMessage message = new TSubMessage();
        if(!lookUpToISubMessageFunc.ContainsKey(message.Descriptor.FullName))
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
                MsgType = subMessage.GameMessageType,
                MsgData = Any.Pack(subMessage)
            }.ToByteArray();
        });
    }

    public IMessage Parse(byte[] rawMessage)
    {
        var dataFromRawMessageWithSizeAppendAhead = rawMessage.GetDataFromRawMessageWithSizeAppendAhead();
        
        var message = GameMessage.Parser.ParseFrom(dataFromRawMessageWithSizeAppendAhead);
        if (message.MsgType == GameMessageType.PingMessage)
        {
            return message;
        }
        else
        {
            if (lookUpToISubMessageFunc.TryGetValue(Any.GetTypeName(message.MsgData.TypeUrl),out var parser))
            {
                var iSubMessage = parser(message.MsgData);
                return lookUpToMessageFunc[iSubMessage.MsgType](iSubMessage);
            }
            else
            {
                throw new UnKnownServerMessageTypeException(message.ToString());
            }
        }
    }

    public byte[] ToByteArray(IMessage message)
    {
        if (message is GameMessage gameMessage)
        {
            return gameMessage.ToByteArray();
        }
        else
        {
            return serializeLookUpFunc[message.GetType()](message);    
        }
    }
}