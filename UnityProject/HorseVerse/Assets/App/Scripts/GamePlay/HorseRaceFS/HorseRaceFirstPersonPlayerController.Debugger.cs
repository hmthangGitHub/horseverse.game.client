﻿using System;
using UnityEngine;

public partial class HorseRaceFirstPersonPlayerController
{
    private static readonly Vector4[] s_UnitSphere = MakeUnitSphere(16);
    
    public static void DrawSphere(Vector4 pos,
                                  float radius,
                                  Color color)
    {
        Vector4[] v = s_UnitSphere;
        int len = s_UnitSphere.Length / 3;
        for (int i = 0; i < len; i++)
        {
            var sX = pos + radius * v[0 * len + i];
            var eX = pos + radius * v[0 * len + (i + 1) % len];
            var sY = pos + radius * v[1 * len + i];
            var eY = pos + radius * v[1 * len + (i + 1) % len];
            var sZ = pos + radius * v[2 * len + i];
            var eZ = pos + radius * v[2 * len + (i + 1) % len];
            Debug.DrawLine(sX, eX, color);
            Debug.DrawLine(sY, eY, color);
            Debug.DrawLine(sZ, eZ, color);
        }
    }
    
    private static Vector4[] MakeUnitSphere(int len)
    {
        Debug.Assert(len > 2);
        var v = new Vector4[len * 3];
        for (int i = 0; i < len; i++)
        {
            var f = i / (float)len;
            float c = Mathf.Cos(f * (float)(Math.PI * 2.0));
            float s = Mathf.Sin(f * (float)(Math.PI * 2.0));
            v[0 * len + i] = new Vector4(c, s, 0, 1);
            v[1 * len + i] = new Vector4(0, c, s, 1);
            v[2 * len + i] = new Vector4(s, 0, c, 1);
        }
        return v;
    }

    public static void DrawPoint(Vector4 pos,
                                 float scale,
                                 Color color)
    {
        var sX = pos + new Vector4(+scale, 0, 0);
        var eX = pos + new Vector4(-scale, 0, 0);
        var sY = pos + new Vector4(0, +scale, 0);
        var eY = pos + new Vector4(0, -scale, 0);
        var sZ = pos + new Vector4(0, 0, +scale);
        var eZ = pos + new Vector4(0, 0, -scale);
        Debug.DrawLine(sX, eX, color);
        Debug.DrawLine(sY, eY, color);
        Debug.DrawLine(sZ, eZ, color);
    }
}