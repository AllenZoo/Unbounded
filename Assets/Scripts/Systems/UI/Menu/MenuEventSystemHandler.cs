using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.Events;
//using UnityEngine.InputSystem;

public class MenuEventSystemHandler : MonoBehaviour
{
    [Header("References")]
    public List<Selectable> Selectables = new List<Selectable>();
    [SerializeField] protected Selectable firstSelected;

    // NOTE: original script includes using UnityEngine.InputSystem making our script more flexible
    //       and work with other inputs (not just mouse). Uncomment and watch this video to redo (just need to uncomment navigate code tbh):
    //       https://www.youtube.com/watch?v=0EsrYNAAEEY&t=691s&ab_channel=SasquatchBStudios
    //       Need Unity 6.
    //[Header("Controls")]
    //[SerializeField] protected InputActionReference navigateReference;

    [Header("Animations")]
    [SerializeField] protected float selectedAnimationScale = 1.1f;
    [SerializeField] protected float scaleDuration = 0.25f;
    [SerializeField] protected List<GameObject> animationExclusions = new List<GameObject>();

    [Header("Sounds")]
    [SerializeField] protected UnityEvent SoundEvent;

    protected Dictionary<Selectable, Vector3> scales = new Dictionary<Selectable, Vector3>();

    protected Selectable lastSelected;

    protected Tween scaleUpTween;
    protected Tween scaleDownTween;

    public virtual void Awake()
    {
        foreach (var selectable in Selectables)
        {
            AddSelectionListeners(selectable);
            scales.Add(selectable, selectable.transform.localScale);
        }
    }

    public virtual void OnEnable()
    {
        //navigateReference.action.performed += OnNavigate;
        // Ensure all selectables are reset back to original size
        for (int i = 0; i < Selectables.Count; i++)
        {
            Selectables[i].transform.localScale = scales[Selectables[i]];
        }

        if (firstSelected != null)
        {
            StartCoroutine(SelectAfterDelay());
        }
    }

    protected  virtual IEnumerator SelectAfterDelay()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
    }

    public virtual void OnDisable()
    {
        //navigateReference.action.performed -= OnNavigate;
        scaleUpTween.Kill(true);
        scaleDownTween.Kill(true);
    }

    public void RegisterSelectable(Selectable selectable)
    {
        if (!Selectables.Contains(selectable))
        {
            Selectables.Add(selectable);
            AddSelectionListeners(selectable);
            scales[selectable] = selectable.transform.localScale;
        }
    }


    protected virtual void AddSelectionListeners(Selectable selectable)
    {
        // Add Listener
        EventTrigger trigger = selectable.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = selectable.gameObject.AddComponent<EventTrigger>();
        }

        // Add SELECT event
        EventTrigger.Entry SelectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Select };
        SelectEntry.callback.AddListener(OnSelect);
        trigger.triggers.Add(SelectEntry);

        // Add DESELECT event
        EventTrigger.Entry DeselectEntry = new EventTrigger.Entry { eventID = EventTriggerType.Deselect };
        DeselectEntry.callback.AddListener(OnDeselect);
        trigger.triggers.Add(DeselectEntry);

        // Add ONPOINTERENTER event
        EventTrigger.Entry PointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        PointerEnter.callback.AddListener(OnPointerEnter);
        trigger.triggers.Add(PointerEnter);

        // Add ONPOINTEREXIT event
        EventTrigger.Entry PointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        PointerExit.callback.AddListener(OnPointerExit);
        trigger.triggers.Add(PointerExit);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SoundEvent?.Invoke();
        lastSelected = eventData.selectedObject.GetComponent<Selectable>();

        if (animationExclusions.Contains(eventData.selectedObject)) return;

        Vector3 newScale = eventData.selectedObject.transform.localScale * selectedAnimationScale;
        scaleUpTween = eventData.selectedObject.transform.DOScale(newScale, scaleDuration);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (animationExclusions.Contains(eventData.selectedObject)) return;

        Selectable sel = eventData.selectedObject.GetComponent<Selectable>();
        scaleDownTween = eventData.selectedObject.transform.DOScale(scales[sel], scaleDuration);
    }

    public void OnPointerEnter(BaseEventData eventData)
    {
        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData !=  null)
        {
            Selectable sel = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
            if (sel == null)
            {
                sel = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
            }

            pointerEventData.selectedObject = sel.gameObject;
        }
    }

    public void OnPointerExit(BaseEventData eventData)
    {
        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData != null)
        {
            pointerEventData.selectedObject = null;
        }
    }

    //protected virtual void OnNavigate(InputAction.CallbackContext context)
    //{
    //    if (EventSystem.current.currentSelectedGameObject == null && lastSelected != null)
    //    {
    //        EventSystem.current.SetSelectedGameObject(lastSelected.gameObject);
    //    }
    //}
}
