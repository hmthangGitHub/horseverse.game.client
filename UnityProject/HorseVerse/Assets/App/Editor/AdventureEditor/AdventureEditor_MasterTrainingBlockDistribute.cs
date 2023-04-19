using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureEditor_MasterTrainingBlockDistribute
{
	public long MasterTrainingBlockDistributeId;
	public int Difficulty;
	public int MasterHorseTrainingBlockGroupId;
	public int Weight;

	public AdventureEditor_MasterTrainingBlockDistribute(MasterTrainingBlockDistribute c)
    {
		MasterTrainingBlockDistributeId = c.MasterTrainingBlockDistributeId;
		Difficulty = c.Difficulty;
		MasterHorseTrainingBlockGroupId = c.MasterHorseTrainingBlockGroupId;
		Weight = c.Weight;
	}

	public void CopyTo(ref MasterTrainingBlockDistribute c)
	{
#if ENABLE_DEBUG_MODULE
		c.Set(MasterTrainingBlockDistributeId, Difficulty,
			MasterHorseTrainingBlockGroupId, Weight);
#endif
	}
}
