using System.Linq;
using io.hverse.game.protogen;

public static class UIComponentRaceRewardGroupFactory
{
    public static UIComponentRaceRewardGroup.Entity CreateRewardGroup(int rank, RacingRoomType racingRoomType)
    {
        return new UIComponentRaceRewardGroup.Entity()
        {
            chestNumber = GetRewardAmount(RewardType.Chest, rank, racingRoomType),
            coinNumber = GetRewardAmount(RewardType.Chip, rank, racingRoomType)
        };
    }

    private static int GetRewardAmount(RewardType rewardType,
                                      int rank,
                                      RacingRoomType racingRoomType)
    {
        UserSettingLocalRepository.MasterDataModel.RacingRewardInfos.TryGetValue((racingRoomType, rank),
            out var rewardGroup);
        return (int)(rewardGroup?.FirstOrDefault(x => x.Type == rewardType)?.Amount ?? 0);
    }
}
