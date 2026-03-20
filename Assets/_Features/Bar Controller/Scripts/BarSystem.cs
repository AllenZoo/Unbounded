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


    // For now we default to HP.
    //[SerializeField, Required] private BarTrackStat statToTrack;

    [Tooltip("Config on whether or not to display the value text (e.g. 100/100) on the bar. For now, this is just a config variable that is not yet implemented.")]
    [SerializeField] private bool displayValueText = false;

    // Fetched References.
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
        controller = new BarController.Builder().WithConfig(displayValueText).Build(barView);
        statModResBinding = new LocalEventBinding<OnStatChangeEvent>(OnStatChange);
    }

    protected void OnEnable()
    {
        barChannel.OnBarContextChange += OnBarChannelUpdate;
    }

    protected void OnDisable()
    {
        barChannel.OnBarContextChange -= OnBarChannelUpdate;
    }

    private void OnBarChannelUpdate()
    {
        // Fetch latest updated barChannel values.
        var stat = barChannel.Stat;
        var leh = barChannel.LEH;

        // Reset system before updating to avoid duplicate event registrations and wrong stat references.
        ResetState();

        // Update system state.
        UpdateState(stat, leh);
    }

    private void UpdateState(StatComponent statObject, LocalEventHandler leh)
    {
        leh.Register(statModResBinding);
    }

    private void ResetState()
    {
        leh?.Unregister(statModResBinding);
        leh = null;
    }

    #region Helpers

    protected void OnStatChange(OnStatChangeEvent e)
    {
        // Update controller, whenever stat value (e.g. HP changes) changes, to update the view.
        controller.UpdateState(barChannel); 
    }  
    #endregion

}
