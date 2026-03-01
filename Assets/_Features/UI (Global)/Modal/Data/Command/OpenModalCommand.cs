using UnityEngine;

[CreateAssetMenu(fileName = "OpenModalCommand", menuName = "System/UI Commands/Open Modal Command", order = 1)]
public class OpenModalCommand : UITriggerCommand
{
    [SerializeField] private ModalContext context;
    [SerializeField] private ModalData data;

    public override void Execute()
    {
        context.Open(data);
    }

    public override void Undo()
    {
        context.Close();
    }
}
