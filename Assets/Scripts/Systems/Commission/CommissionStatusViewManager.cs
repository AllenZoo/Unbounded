using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Displays gameobjects and UI objects based on CommissionStatusType
/// </summary>
public class CommissionStatusViewManager : SerializedMonoBehaviour
{
    [SerializeField] private Dictionary<CommissionViewStatusType, List<GameObject>> objViews;

    /// <summary>
    /// Scriptable Object as Shared Reference between this Renderer and Modifier.
    /// </summary>
    [SerializeField] private CommissionViewStatus commissionViewStatus;

    private void Awake()
    {
        InitView();
    }

    private void Start()
    {
        commissionViewStatus.OnStatusChanged += RenderView;
    }

    /// <summary>
    /// Sets the view based on the CommissionBoardViewStatus.
    /// Disables previous view and enables the new view.
    /// </summary>
    /// <param name="status"></param>
    private void RenderView(CommissionStatusChangedEvent status)
    {
        if (status.prevStatus == status.newStatus)
        {
            return;
        }

        foreach (GameObject obj in objViews[status.prevStatus])
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in objViews[status.newStatus])
        {
            obj.SetActive(true);
        }
    }

    /// <summary>
    /// Initializes the first view. Assumes there is no previous view yet. Disables all views except the first one.
    /// </summary>
    private void InitView()
    {
        // Disable all views except the first one.
        foreach (CommissionViewStatusType status in objViews.Keys)
        {
            if (status != commissionViewStatus.GetStatus())
            {
                foreach (GameObject obj in objViews[status])
                {
                    obj.SetActive(false);
                }
            }
        }

        // Enables the first view.
        foreach (GameObject obj in objViews[commissionViewStatus.GetStatus()])
        {
            obj.SetActive(true);
        }
    }
}
