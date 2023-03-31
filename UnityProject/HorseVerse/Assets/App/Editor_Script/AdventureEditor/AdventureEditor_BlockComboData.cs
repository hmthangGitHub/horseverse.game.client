using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureEditor_BlockComboData : MonoBehaviour
{
    public long id;
    public string block_name;
    public GameObject startPadding;
    public GameObject endPadding;
    public List<GameObject> paddings = new List<GameObject>();
    public List<GameObject> obstabcles = new List<GameObject>();
}