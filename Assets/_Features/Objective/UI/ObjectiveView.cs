using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Main View for the ObjectiveSystemUI. It handles the header and provides
/// an entry point to manage the list of dynamic task items.
/// </summary>
public class ObjectiveView : MonoBehaviour
{
    [Required, SerializeField] private UIDocument objectiveUIDocument;
    [Required, SerializeField] private VisualTreeAsset taskItemTemplate;

    private Label _titleLabel;
    private Label _subtitleLabel;
    private VisualElement _tasksContainer;

    private void Awake()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        if (objectiveUIDocument == null) objectiveUIDocument = GetComponent<UIDocument>();
        if (objectiveUIDocument != null && objectiveUIDocument.rootVisualElement != null)
        {
            var root = objectiveUIDocument.rootVisualElement;
            _titleLabel = root.Q<Label>("Title");
            _subtitleLabel = root.Q<Label>("Subtitle");
            _tasksContainer = root.Q<VisualElement>("Tasks");
        }
    }

    /// <summary>
    /// Main entry point for updating the view. It takes in a config object that contains all necessary information to populate the header and task list.
    /// </summary>
    /// <param name="config"></param>
    public void UpdateView(ObjectiveViewConfig config)
    {
        if (config == null) return;
        SetHeader(config.HeaderTitle, config.HeaderSubtitle);
        ClearTasks();
        foreach (var task in config.TaskItems)
        {
            AddTask(task, taskItemTemplate);
        }
    }

    private void SetHeader(string title, string subtitle)
    {
        if (_titleLabel == null) InitializeUI();
        if (_titleLabel != null) _titleLabel.text = title;
        if (_subtitleLabel != null) _subtitleLabel.text = subtitle;
    }

    private VisualElement AddTask(TaskItemConfig task, VisualTreeAsset template)
    {
        if (_tasksContainer == null) InitializeUI();
        if (_tasksContainer == null) return null;

        var container = template.CloneTree();
        var taskItem = container.Q<VisualElement>(className: "objective-task");
        if (taskItem == null) return null;

        UpdateTaskElement(taskItem, task);
        _tasksContainer.Add(taskItem);
        return taskItem;
    }

    private void UpdateTaskElement(VisualElement element, TaskItemConfig task)
    {
        if (task == null) return;

        var label = element.Q<Label>("TaskLabel");
        if (label != null) label.text = task.TaskText;

        var check = element.Q<VisualElement>("Check");
        if (check != null)
            check.style.display = (task.IsComplete) ? DisplayStyle.Flex : DisplayStyle.None;

        if (!task.IsComplete)
            element.RemoveFromClassList("disabled");
        else
            element.AddToClassList("disabled");
    }

    private void ClearTasks()
    {
        _tasksContainer?.Clear();
    }
}