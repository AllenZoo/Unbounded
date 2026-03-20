using UnityEngine;

/// <summary>
/// Type T is a DTO intermediary between controller and view. (Usually created in controller layer)
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IView<T> where T : class
{
    Canvas DisplayCanvas { get; }
    void ShowView();
    void HideView();
    void UpdateView(T config);
}
