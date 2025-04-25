using UnityEngine;
using UnityEngine.Assertions;

// Intended to be a callback from the Damageable component's TakeDamage event and spawn a damage number for user clarity.
public class DamageNumberSpawner : MonoBehaviour
{
    [Tooltip("The prefab to spawn when damage is taken.")]
    [SerializeField] private GameObject damageNumberPfb;
    [SerializeField] private LocalEventHandler localEventHandler;

    private void Awake()
    {
        // Check that damageNumberPfb has a DamageNumber component
        Assert.IsNotNull(damageNumberPfb.GetComponent<DamageNumber>(), "DamageNumberSpawner needs a prefab with a DamageNumber component.");

        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassigned and not found in parent for object [" + gameObject +
                                       "] with root object [" + gameObject.transform.root.name + "] for DamageNumberSpawner.cs");
            }
        }
    }

    private void Start()
    {
        LocalEventBinding<OnDamagedEvent> onDamagedEventBinding = new LocalEventBinding<OnDamagedEvent>(SpawnDamageNumber);
        localEventHandler.Register(onDamagedEventBinding);
    }

    private void SpawnDamageNumber(OnDamagedEvent e)
    {
        GameObject damageNumber = Instantiate(damageNumberPfb, transform.position, Quaternion.identity, transform);
        damageNumber.GetComponent<DamageNumber>().SetDamageNumber(e.damage);
        damageNumber.GetComponent<DamageNumber>().StartDestroyTimer();
    }
}
