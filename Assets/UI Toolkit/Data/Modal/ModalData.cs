using UnityEngine;

[CreateAssetMenu(fileName = "new ModalData", menuName = "System/UI Toolkit/Modal Data", order = 1)]
public class ModalData : UIContextPayload
{
    public string modalTitle;
    public Sprite modalContentImage;
    public string modalContentMessage;
}
