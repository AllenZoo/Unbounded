using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : Singleton<PlayerSingleton>
{
    public StatComponent GetPlayerStatComponent()
    {
        return GetComponentInChildren<StatComponent>();
    }
}
