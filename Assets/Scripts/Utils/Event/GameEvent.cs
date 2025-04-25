using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Event", menuName = "ScriptableObjs/Game Event Generic 1 Param")]
public class GameEvent<T> : ScriptableObject
{
    public List<GameEventListener<T>> listeners = new List<GameEventListener<T>>();

    public void AddListener(GameEventListener<T> listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(GameEventListener<T> listener)
    {
        listeners.Remove(listener);
    }


    public void Trigger(T t)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(t);
        }
    }
}

// Non-Generic
[CreateAssetMenu(fileName = "New Game Event", menuName = "ScriptableObjs/Game Event Non-Generic")]
public class GameEvent : ScriptableObject
{
    public List<GameEventListener> listeners = new List<GameEventListener>();

    public void AddListener(GameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }


    public void Trigger()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }
}
