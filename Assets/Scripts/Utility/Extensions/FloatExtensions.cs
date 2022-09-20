using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions
{
    public static bool ApproximatelyEquals(this float number, float otherNumber, float tolerance)
    {
        return number > otherNumber - tolerance && number < otherNumber + tolerance;
    }
}