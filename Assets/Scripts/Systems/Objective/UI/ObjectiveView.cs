using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// A view component for displaying the UI elements of a ObjectiveView prefab.
///
/// This class is responsible for configuring the visual elements such as the title,
/// icon (with rotation), and description based on the provided <see cref="Objective"/>.
/// It should be attached to the corresponding UI prefab in the Unity Editor.
/// </summary>
/// <remarks>
/// Expects serialized references to UI elements (TextMeshProUGUI for title and description).
/// The <see cref="SetData"/> method should be called with valid data to populate the card.
/// </remarks>
public class ObjectiveView : MonoBehaviour
{
    [SerializeField, Required] private TextMeshProUGUI titleText;
    [SerializeField, Required] private TextMeshProUGUI descText;
    //private Objective data;

    public void SetData(Objective obj)
    {
        var data = obj.GetData();
        this.titleText.text = data.ObjectiveName;
        this.descText.text = data.ObjectiveText;
    }
}
