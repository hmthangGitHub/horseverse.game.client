using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PlatformModular 
{
    private BlockObjectData GenerateTurn(GameObject turnPrefab)
    {
        var turn = Instantiate(turnPrefab, this.blockContainer);
        return turn.GetComponent<BlockObjectData>();
    }


}
