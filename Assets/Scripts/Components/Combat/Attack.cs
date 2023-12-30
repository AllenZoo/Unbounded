using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script attached to attack objects that contain info about the attack.
[RequireComponent(typeof(Collider2D))]
public class Attack : MonoBehaviour
{
    public float damage = 5f;
    public Boolean isAOE = false;
}
