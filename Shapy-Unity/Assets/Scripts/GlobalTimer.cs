using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlobalTimer
{
    public DateTime startTime;

    public DateTime GetCurrentTimestamp(out DateTime dt)
    {
        dt = DateTime.Now;
        return dt;
    }
    public TimeSpan CalculateTimeDifference(DateTime dt1, DateTime dt2, out TimeSpan diff)
    {
        diff = dt2 - dt1;
        return diff;
    }
}
      