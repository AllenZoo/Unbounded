using System;
using UnityEngine;

public class PauseToken : IDisposable
{
    public Guid guid;
    public PauseManager manager;
    public bool isReleased;

    public void Dispose()
    {
        if (isReleased) return;

        isReleased = true;
        manager.ReleasePause(guid);
    }
}
