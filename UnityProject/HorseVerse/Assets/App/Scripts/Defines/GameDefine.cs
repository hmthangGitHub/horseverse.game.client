using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDefine 
{
    public static string TOKEN_STORAGE = "USER_LOGIN_ACCESS_TOKEN";
    public static string TOKEN_CURRENT_KEY_INDEX = "USER_LOGIN_ACCESS_TOKEN_INDEX";
    public static string AUDIO_MASTER_SOUND = "AUDIO_MASTER_SOUND";
    public static string AUDIO_MUSIC_SOUND = "AUDIO_MUSIC_SOUND";
    public static string AUDIO_SFX_SOUND = "AUDIO_SFX_SOUND";
    public static string AUDIO_MUSIC_MUTE = "AUDIO_MUSIC_MUTE";
    public static string AUDIO_SFX_MUTE = "AUDIO_SFX_MUTE";
}

public enum FEATURE_TYPE
{
    SHOP = 0,
    ADVENTURE = 1,
    ARENA = 2,
    RACING = 3,
}


public enum TYPE_OF_BLOCK
{
    NORMAL = 0,
    START = 1,
    END = 2,
    START_SCENE = 3,
    END_SCENE = 4,
    TURN_LEFT = 5,
    TURN_RIGHT = 6,
    SPLIT_LANES = 7
}