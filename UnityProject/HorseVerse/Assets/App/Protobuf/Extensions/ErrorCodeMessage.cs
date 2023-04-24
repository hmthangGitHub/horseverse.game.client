using UnityEngine.Experimental.TerrainAPI;

namespace io.hverse.game.protogen
{
    public sealed partial class LoginResponse : IErrorCodeMessage {}
    public sealed partial class CancelBettingResponse : IErrorCodeMessage {}
    public sealed partial class GetCurrentRaceScriptResponse : IErrorCodeMessage {}
    public sealed partial class CleanMailsResponse : IErrorCodeMessage {}
    public sealed partial class CombinePiecesResponse : IErrorCodeMessage {}
    public sealed partial class EmailCodeResponse : IErrorCodeMessage {}
    public sealed partial class ExitRoomResponse : IErrorCodeMessage {}
    public sealed partial class GetRaceHistoryResponse : IErrorCodeMessage {}
    public sealed partial class JoinRoomResponse : IErrorCodeMessage {}
    public sealed partial class MailboxInfoResponse : IErrorCodeMessage {}
    public sealed partial class MailRewardsResponse : IErrorCodeMessage {}
    public sealed partial class OpenChestResponse : IErrorCodeMessage {}
    // public sealed partial class PlayerInfoResponse : IErrorCodeMessage {}
    // public sealed partial class PlayerInventoryResponse : IErrorCodeMessage {}
    // public sealed partial class PrivateChatResponse : IErrorCodeMessage {}
    // public sealed partial class PrivateChatResponse : IErrorCodeMessage {}
    // public sealed partial class RaceScriptResponse : IErrorCodeMessage {}
    public sealed partial class ReadMailResponse : IErrorCodeMessage {}
    public sealed partial class StartTrainingResponse : IErrorCodeMessage {}
    public sealed partial class FinishTrainingResponse : IErrorCodeMessage { }
    // public sealed partial class AcceptPrivateChatResponse : IErrorCodeMessage {}
    public sealed partial class ExchangeChestKeyResponse : IErrorCodeMessage {}
    public sealed partial class BetHistoryResponse : IErrorCodeMessage {}
    public sealed partial class SendBettingInfoResponse : IErrorCodeMessage {}
    public sealed partial class BetHistoryDetailResponse : IErrorCodeMessage {}
    public sealed partial class BetHorseListResponse : IErrorCodeMessage {}
    // public sealed partial class GetCurrentBetMatchResponse : IErrorCodeMessage {}
    public sealed partial class GetCurrentRaceScriptResponse : IErrorCodeMessage {}
    public sealed partial class RestartGamePopUpMessage : IErrorCodeMessage {}
    public sealed partial class UpdateRoomResponse : IErrorCodeMessage {}
    public sealed partial class StartRoomResponse : IErrorCodeMessage {}
    public sealed partial class PlayerHorseBasicResponse : IErrorCodeMessage
    {
        public int ResultCode => ErrorCode;
    }
    public sealed partial class PlayerHorseRisingResponse : IErrorCodeMessage
    {
        public int ResultCode => ErrorCode;
    }
    public sealed partial class PlayerHorseHistoryResponse : IErrorCodeMessage
    {
        public int ResultCode => ErrorCode;
    }
    public sealed partial class PlayerHorseAttributeResponse : IErrorCodeMessage
    {
        public int ResultCode => ErrorCode;
    }
    public sealed partial class BreedingResponse : IErrorCodeMessage {}
    public sealed partial class BreedingInfoResponse : IErrorCodeMessage {}
    public sealed partial class FinishBreedingResponse : IErrorCodeMessage {}
    public sealed partial class CheatPlayerInfoResponse : IErrorCodeMessage {}
    public sealed partial class CheatHorseInfoResponse : IErrorCodeMessage {}
}
