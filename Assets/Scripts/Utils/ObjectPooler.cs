using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IObjectPooler<T> where T: MonoBehaviour
{
    T GetObject();
    void ResetObjects();
}

/// <summary>
/// Handles the pooling of objects to reduce the overhead of creating and destroying objects.
/// </summary>
public class ObjectPooler<T>: SerializedMonoBehaviour, IObjectPooler<T> where T : MonoBehaviour
{
    private Transform poolParent;
    private T pfb;
    private List<T> activeList;
    private List<T> inactiveList;

    /// <summary>
    /// Constructor for ObjectPooler.
    /// Object Pooler when initialized will disable and move any objects of type T under the poolParent into the inactive list.
    /// </summary>
    /// <param name="poolParent"></param>
    /// <param name="pfb"></param>
    public ObjectPooler(T pfb, Transform poolParent)
    {
        this.poolParent = poolParent;
        this.pfb = pfb;
        activeList = new List<T>();
        inactiveList = new List<T>();
        Init();
    }

    /// <summary>
    /// Gets an object from pool if available, otherwise instantiates a new one.
    /// </summary>
    /// <returns></returns>
    public T GetObject()
    {
        if (inactiveList.Count > 0)
        {
            T obj = inactiveList[0];
            inactiveList.RemoveAt(0);
            activeList.Add(obj);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            T obj = Instantiate(pfb, poolParent);
            activeList.Add(obj);
            obj.gameObject.SetActive(true);
            return obj;
        }
    }

    /// <summary>
    /// Moves all objects from the active list to the inactive list. 
    /// Disables all objects.
    /// </summary>
    public void ResetObjects()
    {
        foreach (var obj in activeList)
        {
            obj.gameObject.SetActive(false);
            inactiveList.Add(obj);
        }
        activeList.Clear();
    }


    /// <summary>
    /// Disables and moves any objects of type T initially under the parent pool into the inactive list.
    /// </summary>
    private void Init()
    {
        foreach (Transform child in poolParent)
        {
            T obj = child.GetComponent<T>();
            if (obj != null)
            {
                obj.gameObject.SetActive(false);
                inactiveList.Add(obj);
            }
        }
        ResetObjects();
    }
}

/// <summary>
/// Handles the pooling of objects to reduce the overhead of creating and destroying objects.
/// ConsistentOrderObjectPooler always returns the same object in the same order it was added to the pool.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ConsistentOrderObjectPooler<T> : SerializedMonoBehaviour, IObjectPooler<T> where T : MonoBehaviour
{
    private Transform poolParent;
    private T pfb;
    private List<T> allObjs;
    private List<T> activeList;
    private List<T> inactiveList;

    /// <summary>
    /// Constructor for ObjectPooler.
    /// Object Pooler when initialized will disable and move any objects of type T under the poolParent into the inactive list.
    /// ConsistentOrderObjectPooler will remember the order it distributed the objs and keep it the same.
    /// </summary>
    /// <param name="poolParent"></param>
    /// <param name="pfb"></param>
    public ConsistentOrderObjectPooler(T pfb, Transform poolParent)
    {
        this.poolParent = poolParent;
        this.pfb = pfb;
        activeList = new List<T>();
        inactiveList = new List<T>();
        allObjs = new List<T>();
        Init();
    }

    /// <summary>
    /// Gets an object from pool if available, otherwise instantiates a new one.
    /// </summary>
    /// <returns></returns>
    public T GetObject()
    {
        if (inactiveList.Count > 0)
        {
            T obj = inactiveList[0];
            inactiveList.RemoveAt(0);
            activeList.Add(obj);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            T obj = Instantiate(pfb, poolParent);
            activeList.Add(obj);
            allObjs.Add(obj);
            obj.gameObject.SetActive(true);
            return obj;
        }
    }

    /// <summary>
    /// Clears all lists and then moves all objects to the inactive list to maintain order.
    /// Disables all objects.
    /// </summary>
    public void ResetObjects()
    {
        inactiveList.Clear();
        activeList.Clear();
        foreach (var obj in allObjs)
        {
            obj.gameObject.SetActive(false);
            inactiveList.Add(obj);
        }
    }


    /// <summary>
    /// Disables and moves any objects of type T initially under the parent pool into the inactive list and allObjs list.
    /// </summary>
    private void Init()
    {
        foreach (Transform child in poolParent)
        {
            T obj = child.GetComponent<T>();
            if (obj != null)
            {
                obj.gameObject.SetActive(false);
                inactiveList.Add(obj);
                allObjs.Add(obj);
            }
        }
        ResetObjects();
    }
}

