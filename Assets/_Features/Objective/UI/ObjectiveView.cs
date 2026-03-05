using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Main controller for the ObjectiveSystemUI. It handles the header and provides
/// an entry point to manage the list of dynamic task items.
/// </summary>
public class ObjectiveView : MonoBehaviour
{
    [SerializeField] private UIDocument modalUIDocument;
    
    private Label _titleLabel;
    private Label _subtitleLabel;
    private VisualElement _tasksContainer;

    private void Awake()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        if (modalUIDocument == null) modalUIDocument = GetComponent<UIDocument>();
        if (modalUIDocument != null && modalUIDocument.rootVisualElement != null)
        {
            var root = modalUIDocument.rootVisualElement;
            _titleLabel = root.Q<Label>("Title");
            _subtitleLabel = root.Q<Label>("Subtitle");
            _tasksContainer = root.Q<VisualElement>("Tasks");
        }
    }

    public void SetHeader(string title, string subtitle)
    {
        if (_titleLabel == null) InitializeUI();
        if (_titleLabel != null) _titleLabel.text = title;
        if (_subtitleLabel != null) _subtitleLabel.text = subtitle;
    }

    public VisualElement AddTask(Objective obj, VisualTreeAsset template)
    {
        if (_tasksContainer == null) InitializeUI();
        if (_tasksContainer == null) return null;

        var taskItem = template.CloneTree();
        UpdateTaskElement(taskItem, obj);
        _tasksContainer.Add(taskItem);
        return taskItem;
    }

    public void UpdateTaskElement(VisualElement element, Objective obj)
    {
        var data = obj.GetData();
        if (data == null) return;

        var label = element.Q<Label>("TaskLabel");
        if (label != null) label.text = data.ObjectiveText;

        var check = element.Q<VisualElement>("Check");
        if (check != null)
            check.style.display = (obj.GetState() == ObjectiveState.COMPLETE) ? DisplayStyle.Flex : DisplayStyle.None;

        if (obj.GetState() == ObjectiveState.ACTIVE)
            element.RemoveFromClassList("disabled");
        else
            element.AddToClassList("disabled");
    }

    public void ClearTasks()
    {
        _tasksContainer?.Clear();
    }
}