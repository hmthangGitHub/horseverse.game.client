using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailedResponseException : Exception
{
    public int ErrorCode { get; }

    public FailedResponseException(string debugMessage, int errorCode) : base(debugMessage)
    {
        this.ErrorCode = errorCode;
    }
}
