using System.Collections.Generic;

/// <summary>
/// DTO object between ObjectiveController and ObjectiveView.
/// </summary>
public class ObjectiveViewConfig
{
    public string HeaderTitle { get; private set; }
    public string HeaderSubtitle { get; private set; }
    public List<TaskItemConfig> TaskItems { get; private set; }

    public ObjectiveViewConfig(string headerTitle, string headerSubtitle, List<TaskItemConfig> taskItems)
    {
        HeaderTitle = headerTitle;
        HeaderSubtitle = headerSubtitle;
        TaskItems = taskItems;
    }
}

public class TaskItemConfig
{
    public string TaskText { get; private set; }
    public bool IsComplete { get; private set; }
    public TaskItemConfig(string taskText, bool isComplete)
    {
        TaskText = taskText;
        IsComplete = isComplete;
    }
}