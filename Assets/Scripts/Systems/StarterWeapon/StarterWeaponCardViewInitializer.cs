using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Initializes the StarterWeaponCardViewManager with predefined weapon data.
/// 
/// This component is typically used in the scene to wire up and trigger initial data population
/// when the card UI is first displayed.
/// </summary>
/// <remarks>
/// Requires both a <see cref="StarterWeaponCardViewManager"/> and a <see cref="StarterWeaponData"/> reference.
/// </remarks>
[RequireComponent(typeof(StarterWeaponCardViewManager))]
public class StarterWeaponCardViewInitializer : MonoBehaviour
{
    /// <summary>
    /// The card view manager responsible for applying the data to the view.
    /// </summary>
    [Required, SerializeField] private StarterWeaponCardViewManager cardViewManager;

    /// <summary>
    /// The initial weapon data to apply to the card view.
    /// </summary>
    [Required, SerializeField] private StarterWeaponData cardData;

    private void Awake()
    {
        Assert.IsNotNull(cardViewManager);
        Assert.IsNotNull(cardData);

        if (cardViewManager == null) cardViewManager = GetComponent<StarterWeaponCardViewManager>();
    }

    private void Start()
    {
        Init();
    }

    /// <summary>
    /// Initializes the card view with the provided weapon data.
    /// </summary>
    private void Init()
    {
        cardViewManager.SetCardData(cardData);
    }
}
