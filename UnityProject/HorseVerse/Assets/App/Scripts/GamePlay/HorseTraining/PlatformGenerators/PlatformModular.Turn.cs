using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PlatformModular 
{
    private void GenerateTurn(GameObject turnPrefab)
    {
        var turn = Instantiate(turnPrefab, this.blockContainer);
    }

}
