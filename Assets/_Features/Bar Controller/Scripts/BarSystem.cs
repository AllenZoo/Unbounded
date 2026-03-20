using Sirenix.OdinInspector;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Initiates controller (BarController), attaches view to it (BarView), and initializes model (StatComponent - HP).
/// 
/// The main GLUE monobehaviour component.
/// 
/// Main entrypoint of whole system.
/// </summary>
public class BarSystem : MonoBehaviour
{
    #region Model Init Fields
    //[Tooltip("The local event handler belonging to the entity whose stats we are tracking. Could be overriden (eg. bosses) but initial connection here!")]
    //[SerializeField, Required] private LocalEventHandlerContext localEventHandlerContext;
    [SerializeField, Required] public StatComponentContext statContext;
    [SerializeField, Required] private BarChannel barChannel;
    [Tooltip("False for player, true for bosses")]
    [SerializeField] private bool useBarChannel; // false for player, true for bosses


    [SerializeField, Required] private BarTrackStat statToTrack;

    // Fetched References.
    protected StatComponent statObject;
    protected LocalEventHandler leh;

    protected LocalEventBinding<OnStatChangeEvent> statModResBinding;
    #endregion

    #region MVC Fields
    [Required, SerializeField] private BarView barView;

    [FoldoutGroup("Model Initialization")]
    [SerializeField] private bool initStatOnStart = false;
    // Model not always required, could be injected later.
    [FoldoutGroup("Model Initialization")]
    [SerializeField, ShowIf(nameof(initStatOnStart))] private StatContainer model;

    private BarController controller;
    #endregion

    private void Awake()
    {
        Assert.IsNotNull(barView, "Bar System needs a Bar View!");

        if (initStatOnStart)
        {
            Assert.IsNotNull(model, "If you want to initialize the model on start, you need to assign a model!");
            controller = new BarController.Builder()
            .WithModel(model)
            .Build(barView);
        } else
        {
            controller = new BarController.Builder().Build(barView);
        }
    }

    protected void OnEnable()
    {
        localEventHandlerContext.OnInitialized += OnLEHInit;

        if (useBarChannel)
        {
            barChannel.OnBarContextChange += Render;
            barChannel.OnBarContextChange += OnBarContextChange;
        }

        OnLEHInit();
        Render();
    }

    protected void OnDisable()
    {
        localEventHandlerContext.OnInitialized -= OnLEHInit;

        if (useBarChannel)
        {
            barChannel.OnBarContextChange -= Render;
            barChannel.OnBarContextChange -= OnBarContextChange;
        }

        ClearSubscriptions();
    }

    // TODO: subscribe to some event to initialize bar (event passes StatComponent reference)


    #region Helpers

    /// <summary>
    /// Main setter. Clears previous subscriptions from previous LEH and 
    /// handles subscriptions for new one :)
    /// </summary>
    /// <param name="leh"></param>
    public void SetLEH(LocalEventHandler leh)
    {
        ClearSubscriptions();
        this.leh = leh;

        // Temporary fix to fetching the StatComponent.
        // Ideally, the BarController would be initialized with a StatComponent inside some model object,
        // but for now just try to find it in the LEH's gameobject or children.
        if (this.leh != null)
        {
            // Try to find the StatComponent immediately to initialize statObject
            statObject = this.leh.GetComponent<StatComponent>();
            if (statObject == null)
            {
                statObject = this.leh.GetComponentInChildren<StatComponent>();
            }
        }

        Register();
        Render();
    }

    protected void OnStatChange(OnStatChangeEvent e)
    {
        // Update the bar to reflect the new stat
        statObject = e.statComponent;
        Render();
    }

    protected void OnBarContextChange()
    {
        if (leh != null && leh.Equals(barContext.LEH)) return;
        SetLEH(barContext.LEH);
    }

    /// <summary>
    /// Helper function to bind the new LEH.
    /// </summary>
    protected void OnLEHInit()
    {
        if (localEventHandlerContext.Initialized && localEventHandlerContext.LocalEventHandler != null)
        {
            SetLEH(localEventHandlerContext.LocalEventHandler);
        }
    }

    protected void Register()
    {
        if (leh != null)
        {
            // Subscribe Stat Change to OnStatChange()
            statModResBinding = new LocalEventBinding<OnStatChangeEvent>(OnStatChange);
            leh.Register(statModResBinding);
        }
    }

    /// <summary>
    /// Helper to unregister any subscriptions relevant to curent local event handler.
    /// </summary>
    protected void ClearSubscriptions()
    {
        if (leh != null && statModResBinding != null)
        {
            leh.Unregister(statModResBinding);
            statModResBinding = null;
        }

        leh = null;
        statObject = null;
    }


    #endregion

}
