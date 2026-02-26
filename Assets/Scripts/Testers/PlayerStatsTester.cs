using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Script for testing player stats by granting "God Mode" (extreme stats) or custom values.
/// Attach this to a GameObject in your scene and press the specified key to test.
/// 
/// This is for TESTING ONLY - remove from production builds.
/// </summary>
public class PlayerStatsTester : MonoBehaviour
{
    [Header("Test Controls")]
    [Tooltip("Key to toggle God Mode")]
    public KeyCode toggleGodModeKey = KeyCode.G;
    [SerializeField] private bool disableGUI = true;

    [Header("God Mode Stats")]
    [SerializeField] private float godHealth = 999999f;
    [SerializeField] private float godAttack = 999999f;
    [SerializeField] private float godDefense = 999999f;
    [SerializeField] private float godDexterity = 999999f;
    [SerializeField] private float godSpeed = 50f;

    [Header("Status")]
    [ReadOnly, ShowInInspector] private bool isGodMode = false;



    private StatComponent _statComponent;

    private void Update()
    {
        if (Input.GetKeyDown(toggleGodModeKey))
        {
            ToggleGodMode();
        }
    }

    /// <summary>
    /// Toggles God Mode, adding or removing extreme stat modifiers.
    /// </summary>
    [Button("Toggle God Mode")]
    public void ToggleGodMode()
    {
        if (_statComponent == null)
        {
            if (PlayerSingleton.Instance != null)
            {
                _statComponent = PlayerSingleton.Instance.GetPlayerStatComponent();
            }
        }

        if (_statComponent == null)
        {
            Debug.LogError("[PlayerStatsTester] Could not find StatComponent on player. Make sure PlayerSingleton is active.");
            return;
        }

        isGodMode = !isGodMode;

        if (isGodMode)
        {
            ApplyGodStats();
            Debug.Log($"<color=gold>[PlayerStatsTester] God Mode ENABLED! (Press {toggleGodModeKey})</color>");
        }
        else
        {
            RemoveGodStats();
            Debug.Log($"<color=red>[PlayerStatsTester] God Mode DISABLED! (Press {toggleGodModeKey})</color>");
        }
    }

    private void ApplyGodStats()
    {
        var mediator = _statComponent.StatContainer.StatMediator;
        
        // Remove existing modifiers from this source to avoid duplicates
        mediator.RemoveModifiersFromSource(this);

        // Add extreme modifiers
        mediator.AddModifier(this, new StatModifier(Stat.MAX_HP, new AddOperation(godHealth), -1));
        mediator.AddModifier(this, new StatModifier(Stat.ATK, new AddOperation(godAttack), -1));
        mediator.AddModifier(this, new StatModifier(Stat.DEF, new AddOperation(godDefense), -1));
        mediator.AddModifier(this, new StatModifier(Stat.SPD, new AddOperation(godSpeed), -1));
        mediator.AddModifier(this, new StatModifier(Stat.DEX, new AddOperation(godDexterity), -1));

        // Refill health to new max
        _statComponent.StatContainer.Health = _statComponent.StatContainer.MaxHealth;
    }

    private void RemoveGodStats()
    {
        if (_statComponent == null) return;

        var mediator = _statComponent.StatContainer.StatMediator;
        mediator.RemoveModifiersFromSource(this);
        
        // Cap health to new MaxHealth
        if (_statComponent.StatContainer.Health > _statComponent.StatContainer.MaxHealth)
        {
            _statComponent.StatContainer.Health = _statComponent.StatContainer.MaxHealth;
        }
    }

    private void OnGUI()
    {
        if (disableGUI) return;
        // Display test instructions in the game view
        // Positioned to avoid overlap with PlayerDeathTester (150) and GameOverUITester (10)
        GUILayout.BeginArea(new Rect(10, 240, 300, 100));
        GUILayout.Box("Player Stats Tester");
        string status = isGodMode ? "<color=yellow>ENABLED</color>" : "DISABLED";
        GUILayout.Label($"God Mode: {status}");
        GUILayout.Label($"Press {toggleGodModeKey} - Toggle God Mode");
        GUILayout.Label("Or use Inspector button");
        GUILayout.EndArea();
    }
}
