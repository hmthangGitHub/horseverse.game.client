using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "training_block_collection", menuName = "HVerse/Training Block Collection", order = 4)]
public class TrainingBlockCollection : ScriptableObject
{
    public GameObject[] blocks;
    public GameObject[] startingBlocks;
    public GameObject[] endingBlocks;


    private Dictionary<string, GameObject> blocksLookUpTable;
    public IReadOnlyDictionary<string, GameObject> BlocksLookUpTable => blocksLookUpTable ??= blocks.ToDictionary(x => x.name);

    private Dictionary<string, GameObject> staringBlocksLookUpTable;
    public IReadOnlyDictionary<string, GameObject> StartingBlocksLookUpTable => staringBlocksLookUpTable ??= startingBlocks.ToDictionary(x => x.name);

    private Dictionary<string, GameObject> endingBlocksLookUpTable;
    public IReadOnlyDictionary<string, GameObject> EndingBlocksLookUpTable => endingBlocksLookUpTable ??= endingBlocks.ToDictionary(x => x.name);
}
