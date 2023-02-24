using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "training_block_settings", menuName = "HVerse/Training Block Settings ", order = 1)]
public class TrainingBlockSettings : ScriptableObject
{
    public TrainingBlockPredefine[] blockCombos;
    public GameObject[] obstacles;
    public GameObject[] sceneryObjects;
    public GameObject[] blocks;
    public GameObject[] traps;
    public GameObject[] trapEditors;

    private Dictionary<string, GameObject> blocksLookUpTable;
    public IReadOnlyDictionary<string, GameObject> BlocksLookUpTable => blocksLookUpTable ??= blocks.ToDictionary(x => x.name);
}