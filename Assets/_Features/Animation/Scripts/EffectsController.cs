using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class EffectsController : MonoBehaviour
{
    [Required, SerializeField] private LocalEventHandler leh;

    // For changing colour of the sprite (giving it a tint) based on state.
    [Required, SerializeField] private SpriteRenderer sprite;

    // For making the sprite flash white. 
    [SerializeField] private Material damageMaterial;
    [SerializeField] private Material defaultMaterial;

    // For whether entity should make damaged sound effect. (Set enum to 'None' if nothing)
    [SerializeField] private SoundType damagedSoundEffect = SoundType.None;
    private bool runningDamageEffect = false;


    // Information about entity state.
    private State curState;

    private void Awake()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        Assert.IsNotNull(sprite, "SpriteRenderer null in animation controller for object: " + gameObject);

        if (defaultMaterial == null)
        {
            defaultMaterial = new Material(sprite.sharedMaterial);
        }

        Assert.IsNotNull(damageMaterial, "Need material for being damaged");

        leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);
    }

    private void Start()
    { 
        LocalEventBinding<OnStateChangeEvent> stateEventBinding = new LocalEventBinding<OnStateChangeEvent>(HandleOnStateChange);
        leh.Register<OnStateChangeEvent>(stateEventBinding);

        LocalEventBinding<OnDamagedEvent> damagedEventBinding = new LocalEventBinding<OnDamagedEvent>(HandleOnDamagedEvent);
        leh.Register<OnDamagedEvent>(damagedEventBinding);
    }

    private void PlayDamagedEffect(float damage)
    {
        StartCoroutine(DamagedEffect(damage));
    }

    private void SetState(State state)
    {
        this.curState = state;
        HandleEffects(state);
    }

    private void HandleOnStateChange(OnStateChangeEvent e)
    {
        // Do it in this order to ensure that we animation transition State.DEAD.
        SetState(e.newState);
        switch (e.newState)
        {
            case State.DEAD:
                //CanTransitionState = false;
                break;
            default:
                //CanTransitionState = true;
                break;
        }
    }

    private void HandleOnDamagedEvent(OnDamagedEvent e)
    {
        // TODO: could change state based on how much damage is taken. eg. more dmg = more damaged state.
        //       for future enhancements.
        if (curState != State.DEAD)
        {
            PlayDamagedEffect(e.damage);
        }
    }


    // Modifies the sprite colour
    private void HandleEffects(State state)
    {
        if (runningDamageEffect)
        {
            sprite.color = Color.white;
            return;
        }

        switch (state)
        {
            case State.STUNNED:
                sprite.color = new Color(0.745283f, 0.614507f, 0.614507f);
                break;
            case State.DEAD:
                sprite.color = Color.black;
                break;
            default:
                sprite.color = Color.white;
                break;
        }
    }

    private IEnumerator DamagedEffect(float damage)
    {
        runningDamageEffect = true;
        HandleEffects(curState);

        AudioManager.PlaySound(damagedSoundEffect, 1);
        sprite.material = damageMaterial;
        yield return new WaitForSeconds(0.2f);
        sprite.material = defaultMaterial;

        runningDamageEffect = false;
        HandleEffects(curState);
    }
}
