using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IBoostableObject
{
    public abstract void BoostProgress();
    public abstract bool IsInBoostableState();
    public abstract DateTime GetTimeFinished();
    public abstract TimeSpan GetTotalDuration();
}
