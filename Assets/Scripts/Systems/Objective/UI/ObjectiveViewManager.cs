using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Handles the interaction between the <see cref="ObjectiveView"/> and the data model <see cref="Objective"/>.
/// 
/// This component is responsible for receiving objective data and passing it to the view for display.
/// It acts as a controller in a simplified MVC pattern.
/// </summary>
/// <remarks>
/// This component requires a <see cref="ObjectiveView"/> to function correctly.
/// </remarks>
[RequireComponent(typeof(ObjectiveView))]
public class ObjectiveViewManager : MonoBehaviour
{
    [SerializeField, Required] private ObjectiveView view;
    [SerializeField, ReadOnly] private Objective data;

    private void Awake()
    {
        view = GetComponent<ObjectiveView>();
        Assert.IsNotNull(view);
    }

    /// <summary>
    /// Sets the weapon data and updates the card view accordingly.
    /// </summary>
    /// <param name="cardData">The weapon data to apply to the card view.</param>
    public void SetViewData(Objective data)
    {
        this.data = data;
        view.SetData(data);
    }
}
