using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "new ModalData", menuName = "System/UI Toolkit/Modal Data", order = 1)]
public class ModalData : ScriptableObject
{
    public string ModalTitle;
    public Sprite ModalContentImage;
    public string ModalContentMessage;
    public ScriptableObjectBoolean ModalAnswerPayload; // To set the answer payload on confirm/cancel
}
