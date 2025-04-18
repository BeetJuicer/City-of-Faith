
using System;

public interface ICanvasView
{
    public abstract void Activate();
    public abstract void Deactivate();
    public event Action Canvas_Deactivated;
}
