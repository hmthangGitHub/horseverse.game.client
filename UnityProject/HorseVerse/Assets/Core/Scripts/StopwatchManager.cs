using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopwatchLogManager
{
    private StopwatchLogManager()
    {

    }

    private static StopwatchLogManager _instance;
    public static StopwatchLogManager Instance
    {
        get
        {
            return _instance ?? (_instance = new StopwatchLogManager());
        }
    }

    private List<LogTimeData> _timerList = new List<LogTimeData>();
    private float _currentTime = 0;
    private System.Diagnostics.Stopwatch _stopwatch;

    public void Start()
    {
#if !ENSURE_NO_CHEAT
        _timerList.Clear();
        _stopwatch = new System.Diagnostics.Stopwatch();
        _stopwatch.Start();
        _currentTime = 0;
#endif
    }

    public void End()
    {
#if !ENSURE_NO_CHEAT
        if (_stopwatch == null)
            return;
        _stopwatch.Stop();
        Debug.Log("<color=green>====Total time: " + (_stopwatch.ElapsedMilliseconds / 1000f) + "</color>");
        foreach (var log in _timerList)
        {
            Debug.Log("=====" + log.Key + ": " + log.Time + "=====");
        }
        _stopwatch = null;
        _timerList.Clear();
        _currentTime = 0;
#endif
    }

    public void AddLog(string key)
    {
#if !ENSURE_NO_CHEAT
        //Debug.Log("==" + key + ": " + Time.time + "==");
        if (_stopwatch == null)
        {
            Start();
        }
        var time = _stopwatch.ElapsedMilliseconds / 1000f - _currentTime;
        _currentTime = _stopwatch.ElapsedMilliseconds / 1000f;
        _timerList.Add(new LogTimeData { Key = key, Time = time });
#endif
    }

    struct LogTimeData
    {
        public string Key;
        public float Time;
    }
}