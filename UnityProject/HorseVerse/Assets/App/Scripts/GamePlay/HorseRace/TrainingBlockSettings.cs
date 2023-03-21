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
    public GameObject[] turnLeftBlocks;
    public GameObject[] turnRightBlocks;
    public GameObject[] startBlocks;
    public GameObject[] endBlocks;

    public GameObject[] traps;
    public GameObject[] trapEditors;

    private Dictionary<string, GameObject> blocksLookUpTable;
    public IReadOnlyDictionary<string, GameObject> BlocksLookUpTable => blocksLookUpTable ??= blocks.ToDictionary(x => x.name);

    private Dictionary<string, GameObject> turnLeftBlocksLookUpTable;
    public IReadOnlyDictionary<string, GameObject> TurnLeftBlocksLookUpTable => turnLeftBlocksLookUpTable ??= turnLeftBlocks.ToDictionary(x => x.name);

    private Dictionary<string, GameObject> turnRightBlocksLookUpTable;
    public IReadOnlyDictionary<string, GameObject> TurnRightBlocksLookUpTable => turnRightBlocksLookUpTable ??= turnRightBlocks.ToDictionary(x => x.name);

    private Dictionary<string, GameObject> startBlocksLookUpTable;
    public IReadOnlyDictionary<string, GameObject> StartBlocksLookUpTable => startBlocksLookUpTable ??= startBlocks.ToDictionary(x => x.name);

    private Dictionary<string, GameObject> endBlocksLookUpTable;
    public IReadOnlyDictionary<string, GameObject> EndBlocksLookUpTable => endBlocksLookUpTable ??= endBlocks.ToDictionary(x => x.name);
}