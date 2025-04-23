using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOnPlay : MonoBehaviour
{
    [SerializeField, Required] private LocalEventHandler leh;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        leh.Call(new OnDeathEvent());
    }
}
