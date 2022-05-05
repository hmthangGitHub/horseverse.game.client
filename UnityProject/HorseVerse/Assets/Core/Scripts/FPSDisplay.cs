using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour {

    [SerializeField]
    Text FPSDisplayText;

    int m_frameCounter = 0;     float m_timeCounter = 0.0f;     float m_lastFramerate = 0.0f;     float m_refreshTime = 0.3f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (FPSDisplayText != null)         {             if (m_timeCounter < m_refreshTime)             {                 m_timeCounter += Time.unscaledDeltaTime;                 m_frameCounter++;             }             else             {
                //This code will break if you set your m_refreshTime to 0, which makes no sense.
                m_lastFramerate = (float)m_frameCounter / m_timeCounter;                 m_frameCounter = 0;                 m_timeCounter = 0.0f;                 FPSDisplayText.text = (int)m_lastFramerate + " FPS";             }         }
	}
}
