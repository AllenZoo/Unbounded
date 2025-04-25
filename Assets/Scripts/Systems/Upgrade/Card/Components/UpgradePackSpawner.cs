using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class attached to objects that want to spawn an upgrade pack on some local event.
/// </summary>
public class UpgradePackSpawner : MonoBehaviour
{
    [SerializeField] private LocalEventHandler leh;
    [SerializeField] private UpgradeCardDeckData cardDeckData;
    [SerializeField] private int numCardsInPack = 3;
    [SerializeField] private UpgradeCardPack cardPackPfb;
    [SerializeField] private const bool spawnOnLocalDeath = true;

    private LocalEventBinding<OnDeathEvent> deathEventBinding;

    #region Unity Lifecycle Functions
    private void Awake()
    {
        if (leh == null)
        {
            leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(this.gameObject);
            leh = GetComponentInParent<LocalEventHandler>();
        }

        if (spawnOnLocalDeath)
        {
            deathEventBinding = new LocalEventBinding<OnDeathEvent>(OnLocalDeathEvent);
            leh.Register(deathEventBinding);
        }
    }

    private void OnDestroy()
    {
        if (spawnOnLocalDeath)
        {
            leh.Unregister(deathEventBinding);
        }
    }
    #endregion

    private void OnLocalDeathEvent(OnDeathEvent e)
    {
        // Get cards, spawn pack pfb, and then finally set pack pfb cards.
        HashSet<UpgradeCardData> cards = UpgradePackCardCalculator.GetCardsFromDeck(cardDeckData, numCardsInPack);
        var newCardPackPfb = Instantiate(cardPackPfb, gameObject.transform.position, Quaternion.identity);
        newCardPackPfb.SetCards(cards);
    }
}
