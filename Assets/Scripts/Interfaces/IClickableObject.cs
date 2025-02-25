using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickableObject
{
    public abstract void OnObjectClicked();
    public abstract void ResetPopState();
}

