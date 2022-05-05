using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogItem : MonoBehaviour {

    [SerializeField]
    Text LogText;

	// Use this for initialization
	void Start () {
		
	}
	
    public void SetText(string text){
        LogText.text = text;
    }
}
