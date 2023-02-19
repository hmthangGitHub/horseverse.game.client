using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MasterErrorCodeDefine
{
    public static int SUCCESS = 100;
    public static int UNKNOWN_ERROR = 101;
    public static int WRONG_REQUEST = 102;
    public static int LOAD_DATA_FAILED = 103;
    public static int SAVE_DATA_FAILED = 104;
    public static int NOT_ENOUGH = 105;
    public static int NOT_OWN = 106;
    public static int NOT_EXIST = 107;
    public static int ACCOUNT_NOT_ACTIVATE = 200;
    public static int ACCOUNT_NOT_FOUND = 201;
    public static int WRONG_PASSWORD = 202;
    public static int UNAUTHORIZED = 203;
    public static int EXPIRED_TOKEN = 204;
    public static int LOGIN_MULTIPLE_DEVICES = 205;
    public static int NEED_TO_UPDATE_CLIENT = 206;
}