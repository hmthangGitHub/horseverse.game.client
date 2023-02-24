using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapEditorBase : MonoBehaviour
{
    public virtual string GetExtraData() { return ""; }
    public virtual Object ParseData(string data) { return null; }
}
