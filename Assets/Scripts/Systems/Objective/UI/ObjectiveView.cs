using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
