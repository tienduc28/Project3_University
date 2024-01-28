using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilFunc
{
    static System.Random rnd = new System.Random(DateTime.UtcNow.Millisecond);

    public static bool Roll(float chance)
    {
        float ran = rnd.Next(0, 100) / (float)100;
        return ran < chance;
    }

    public static float GetHorizontalSpeed(Vector3 speed)
    {
        return Mathf.Sqrt(speed.x * speed.x + speed.z * speed.z);
    }
}
