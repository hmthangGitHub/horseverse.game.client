using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureEditor_MasterHorseTrainingBlock
{
	public long MasterHorseTrainingBlockId;
	public string Name;
	public MasterHorseTrainingLaneType MasterHorseTrainingLaneTypeLeft;
	public int CustomValueLeft;
	public MasterHorseTrainingLaneType MasterHorseTrainingLaneTypeMid;
	public int CustomValueMid;
	public MasterHorseTrainingLaneType MasterHorseTrainingLaneTypeRight;
	public int CustomValueRight;

	public AdventureEditor_MasterHorseTrainingBlock(MasterHorseTrainingBlock c)
    {
		MasterHorseTrainingBlockId = c.MasterHorseTrainingBlockId;
		Name = c.Name;
		MasterHorseTrainingLaneTypeLeft = c.MasterHorseTrainingLaneTypeLeft;
		CustomValueLeft = c.CustomValueLeft;
		MasterHorseTrainingLaneTypeMid = c.MasterHorseTrainingLaneTypeMid;
		CustomValueMid = c.CustomValueMid;
		MasterHorseTrainingLaneTypeRight = c.MasterHorseTrainingLaneTypeRight;
		CustomValueRight = c.CustomValueRight;
	}

	public void CopyTo(ref MasterHorseTrainingBlock c)
    {
#if ENABLE_DEBUG_MODULE
		c.Set(MasterHorseTrainingBlockId, Name,
			MasterHorseTrainingLaneTypeLeft, CustomValueLeft,
			MasterHorseTrainingLaneTypeMid, CustomValueMid,
			MasterHorseTrainingLaneTypeRight, CustomValueRight);
#endif
	}

#if ENABLE_DEBUG_MODULE
	public MasterHorseTrainingBlock Create()
	{
		return new MasterHorseTrainingBlock(MasterHorseTrainingBlockId, Name,
			MasterHorseTrainingLaneTypeLeft, CustomValueLeft,
			MasterHorseTrainingLaneTypeMid, CustomValueMid,
			MasterHorseTrainingLaneTypeRight, CustomValueRight);
	}
#endif
}
