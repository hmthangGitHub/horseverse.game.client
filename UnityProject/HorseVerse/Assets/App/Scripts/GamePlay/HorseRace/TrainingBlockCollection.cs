using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "training_block_collection", menuName = "HVerse/Training Block Collection", order = 4)]
public class TrainingBlockCollection : ScriptableObject
{
    public GameObject[] blocks;
    private Dictionary<string, GameObject> blocksLookUpTable;
    public IReadOnlyDictionary<string, GameObject> BlocksLookUpTable => blocksLookUpTable ??= blocks.ToDictionary(x => x.name);
}
