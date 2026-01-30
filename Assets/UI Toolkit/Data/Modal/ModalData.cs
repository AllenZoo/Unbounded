using Sirenix.OdinInspector;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new ModalData", menuName = "System/UI Toolkit/Modal Data", order = 1)]
public class ModalData : ScriptableObject
{
    public string ModalTitle;
    public Sprite ModalContentImage;

    [TextArea(3, 8)]
    public string ModalContentMessage;
}
