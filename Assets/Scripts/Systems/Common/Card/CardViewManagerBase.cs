using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class acts as a base for card controller class in the simplified MVC pattern.
/// </summary>
/// <typeparam name="TCardData"></typeparam>
public abstract class CardViewManagerBase <TCardData> : MonoBehaviour
{
    public abstract void SetCardData(TCardData data);
}
