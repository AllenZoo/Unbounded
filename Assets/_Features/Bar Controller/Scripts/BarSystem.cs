using Sirenix.OdinInspector;
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
    [SerializeField] private bool displayValueText = false;

    // Optional context to auto-initialize the channel (mostly for player)
    [SerializeField] private LocalEventHandlerContext localEventHandlerContext;

    protected LocalEventHandler leh;
    protected LocalEventBinding<OnStatChangeEvent> statModResBinding;

    #region MVC Fields
    [SerializeField, Required] private BarChannel barChannel; // Model
    [Required, SerializeField] private BarView barView;
    private BarController controller;
    #endregion

    private void Awake()
    {
        Assert.IsNotNull(barView, "Bar System needs a Bar View!");
        controller = new BarController.Builder().WithModel(barChannel).WithConfig(displayValueText).Build(barView);
        statModResBinding = new LocalEventBinding<OnStatChangeEvent>(OnStatChange);
    }

    protected void OnEnable()
    {
        barChannel.OnBarContextChange += OnBarChannelUpdate;
        
        if (localEventHandlerContext != null)
        {
            localEventHandlerContext.OnInitialized += OnContextInit;
            if (localEventHandlerContext.Initialized) OnContextInit();
        }

        OnBarChannelUpdate();
    }

    protected void OnDisable()
    {
        barChannel.OnBarContextChange -= OnBarChannelUpdate;
        
        if (localEventHandlerContext != null)
        {
            localEventHandlerContext.OnInitialized -= OnContextInit;
        }

        ResetState();
    }

    private void OnContextInit()
    {
        if (localEventHandlerContext.LocalEventHandler != null)
        {
            // Auto-populate the channel from the context
            barChannel.Stat = localEventHandlerContext.LocalEventHandler.GetComponent<StatComponent>();
            barChannel.LEH = localEventHandlerContext.LocalEventHandler;
            barChannel.IsVisible = true;
        }
    }

    private void OnBarChannelUpdate()
    {
        ResetState();

        var stat = barChannel.Stat;
        var newLeh = barChannel.LEH;

        if (stat != null && newLeh != null)
        {
            UpdateState(stat, newLeh);
        }

        controller.UpdateState(barChannel);
    }

    private void UpdateState(StatComponent statObject, LocalEventHandler newLeh)
    {
        this.leh = newLeh;
        this.leh.Register(statModResBinding);
    }

    private void ResetState()
    {
        if (this.leh != null)
        {
            this.leh.Unregister(statModResBinding);
            this.leh = null;
        }
    }

    protected void OnStatChange(OnStatChangeEvent e)
    {
        controller.UpdateState(barChannel); 
    }  
}