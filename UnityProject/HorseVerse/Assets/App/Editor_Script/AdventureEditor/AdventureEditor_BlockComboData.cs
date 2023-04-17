using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureEditor_BlockComboData : MonoBehaviour
{
    public long id;
    public string block_name;
    public int group_id;
    public GameObject startPadding;
    public GameObject endPadding;
    public List<GameObject> paddings = new List<GameObject>();
    public List<GameObject> obstabcles = new List<GameObject>();
    public List<AdventureEditor_CoinEditor> coins = new List<AdventureEditor_CoinEditor>();
    public List<TrapEditor> traps = new List<TrapEditor>();
    public List<GameObject> subObjects = new List<GameObject>();

}