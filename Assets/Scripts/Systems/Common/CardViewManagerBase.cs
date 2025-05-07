using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardViewManagerBase <TCardData> : MonoBehaviour
{
    public abstract void SetCardData(TCardData data);
}
