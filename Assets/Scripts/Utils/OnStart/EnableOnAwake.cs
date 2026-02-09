using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnAwake : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToEnable = new();
    void Awake()
    {
        // Note: this does not work since Unity does not allow enabling a GameObject in Awake if it is disabled.
        //this.gameObject.SetActive(true);

        foreach (var obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}
