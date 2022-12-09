using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundController
{

    public static void SetSFXVolume(float value)
    {
        if(AudioManager.Instance != default)
            AudioManager.Instance.SfxVolume = value;
    }

    public static void SetBGMVolume(float value)
    {
        if (AudioManager.Instance != default)
            AudioManager.Instance.MusicVolume = value;
    }

    public static void SetGFXVolume(float value)
    {
        if (AudioManager.Instance != default)
            AudioManager.Instance.SoundAllVolume = value;
    }

    public static float GetSFXVolume()
    {
        if (AudioManager.Instance != default)
            return AudioManager.Instance.SfxVolume;
        return 0;
    }

    public static float GetBGMVolume()
    {
        if (AudioManager.Instance != default)
            return AudioManager.Instance.MusicVolume;
        return 0;
    }

    public static float GetGFXVolume()
    {
        if (AudioManager.Instance != default)
            return AudioManager.Instance.SoundAllVolume;
        return 0;
    }


    public static void PlayClick()
    {
        AudioManager.Instance?.PlaySound("Click");
    }

    public static void PlayMusicBase()
    {
        AudioManager.Instance?.PlayMusic("Base");
    }

    public static void PlayMusicBetModePrepare()
    {
        AudioManager.Instance?.PlayMusic("BetModePrepare");
    }

    public static void PlayMusicTrainingInGame()
    {
        AudioManager.Instance?.PlayMusic("TrainingInGame");
    }
}
