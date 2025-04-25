using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "new pageUI context", menuName = "System/General UI/PageUIContext")]
public class PageUIContext : ScriptableObject
{
    public PageUI PageUI { get { return initialized ? pageUI : null; } }

    [SerializeField, ReadOnly] private PageUI pageUI;

    [SerializeField, ReadOnly] private bool initialized = false;
    public void Init(PageUI pageUI)
    {
        if (initialized)
        {
            // If we get here, that means we assigned the same scriptable object to two different PageUIs.
            Debug.LogError("Trying to initialize PageUI context twice! Should be assigned to only 1 page UI!");
            return;
        }
        initialized = true;
        this.pageUI = pageUI;
    }

    //TODO call this on editor play end.
    public void ResetContext()
    {
        initialized = false;
        pageUI = null;
    }

    private void OnDisable()
    {
        ResetContext();
    }
}
