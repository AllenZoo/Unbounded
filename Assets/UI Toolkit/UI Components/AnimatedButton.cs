using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class AnimatedButton : VisualElement
{
    [UxmlAttribute]
    public string Text { get; set; }

}
