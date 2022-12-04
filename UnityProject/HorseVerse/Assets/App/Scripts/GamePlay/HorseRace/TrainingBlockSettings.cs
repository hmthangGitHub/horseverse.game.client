using UnityEngine;

[CreateAssetMenu(fileName = "training_block_settings", menuName = "HVerse/Training Block Settings ", order = 1)]
public class TrainingBlockSettings : ScriptableObject
{
    public TrainingBlockPredefine[] blockCombos;
    public GameObject[] obstacles;
    public GameObject[] sceneryObjects;
}