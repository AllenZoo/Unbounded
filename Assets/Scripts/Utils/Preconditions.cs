using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preconditions
{
    public static void CheckNotNull(Object obj, string errMessage)
    {
        Debug.LogError(errMessage);
        throw new System.Exception(errMessage);
    }
}
