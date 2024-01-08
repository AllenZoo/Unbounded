using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Animator))]
public class AnimatorController : MonoBehaviour
{
    // For changing colour of the sprite (giving it a tint) based on state.
    [SerializeField] private SpriteRenderer sprite;

    // For making the sprite flash white. 
    [SerializeField] private Material damageMaterial;

    // For subscribing to entities state changes.
    // TODO: remove. We just call the change state stuff in state component.
    [SerializeField] private StateComponent stateComponent;

    private Material defaultMaterial;
    private bool runningDamageEffect = false;


    // Animation Names
    public static string IDLE_ANIMATION_NAME = "Idle";
    public static string WALKING_ANIMATION_NAME = "Walking";
    public static string RUNNING_ANIMATION_NAME = "Running";
    public static string ATTACKING_ANIMATION_NAME = "Attacking";
    public static string STUNNED_ANIMATION_NAME = "Stunned";

    // Update this whenever a new animation is added.
    // TODO: incorporate this in checking for animation clips.
    private static int ANIMATION_CLIP_COUNT = 5;

    // Animator Parameter Names
    public static string DIRECTION_PARAMETER_X = "xDir";
    public static string DIRECTION_PARAMETER_Y = "yDir";
    public static string LAST_DIRECTION_PARAMETER_X = "last_xDir";
    public static string LAST_DIRECTION_PARAMETER_Y = "last_yDir";

    // Update this whenever new parameter is added.
    public static float NUMBER_OF_PARAMETERS = 4;

    // Components
    [Header("Can be null.")]
    [Tooltip("Can be null. Set if we want to control animations that change based on motion.")]
    [SerializeField] private MotionComponent motionComponent;
    
    private Animator animator;

    // State in this case refers to animation states and not entity states.
    private bool canTransitionState;
    private State lastNonCCState;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        // TODO: remove this, temp.
        if (stateComponent == null)
        {
            stateComponent = GetComponent<StateComponent>();
        }

        if (motionComponent == null)
        {
            // Try seeing if motionComponent is on obj.
            motionComponent = GetComponent<MotionComponent>();
        }

        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        Assert.IsNotNull(sprite, "SpriteRenderer null in animation controller for object: " + gameObject);

        defaultMaterial = sprite.material;

        Assert.IsNotNull(damageMaterial, "Need material for being damaged");
    }

    private void Start()
    {
        Debug.Assert(stateComponent != null, "State null in animation controller for object: " + gameObject);
        Debug.Assert(animator != null, "Animator null in animation controller for object: " + gameObject);
        stateComponent.OnStateChanged += State_OnStateChanged;

        if (motionComponent != null)
        {
            motionComponent.OnMotionChange += Motion_OnMotionChange;

            // Check Parameters are present in animator controller
            Debug.Assert(animator.parameters.Length >= NUMBER_OF_PARAMETERS,
                "Animator parameters not set up correctly for object: " + gameObject);
        }
        
        // Init Parameters in animator (TODO: once it gets large, move to helper)
        animator.SetFloat(DIRECTION_PARAMETER_X, 0);
        animator.SetFloat(DIRECTION_PARAMETER_Y, 0);
        animator.SetFloat(LAST_DIRECTION_PARAMETER_X, 0);
        animator.SetFloat(LAST_DIRECTION_PARAMETER_Y, 0);
    }

    public void PlayDamagedEffect(float damage)
    {
        StartCoroutine(DamagedEffect(damage));
    }

    private void Motion_OnMotionChange(Vector2 dir, Vector2 lastDir)
    {
        SetMovementParameters(dir, lastDir);
    }

    private void State_OnStateChanged(State oldState, State newState)
    {
        if (!stateComponent.GetCCStates.Contains(newState))
        {
            lastNonCCState = newState;
        }

        Handle_Animation(stateComponent.state);
        Handle_Effects(stateComponent.state);
    }

    private void SetMovementParameters(Vector2 dir, Vector2 lastDir)
    {
        animator.SetFloat(DIRECTION_PARAMETER_X, dir.x);
        animator.SetFloat(DIRECTION_PARAMETER_Y, dir.y);
        animator.SetFloat(LAST_DIRECTION_PARAMETER_X, lastDir.x);
        animator.SetFloat(LAST_DIRECTION_PARAMETER_Y, lastDir.y);
    }

    // Modifies the state of the animator 
    private void Handle_Animation(State state)
    {
        switch (state)
        {
            case State.IDLE:
                animator.Play(IDLE_ANIMATION_NAME);
                break;
            case State.WALKING:
                animator.Play(WALKING_ANIMATION_NAME);
                break;
            case State.RUNNING:
                animator.Play(RUNNING_ANIMATION_NAME);
                break;
            case State.STUNNED:
                animator.Play(STUNNED_ANIMATION_NAME);
                break;
            case State.ATTACKING:
                animator.Play(ATTACKING_ANIMATION_NAME);
                break;
            default:
                Debug.LogWarning("Implement animator for state: " + state.ToString());
                //Debug.LogError("Encountered invalid state: " + state.state.ToString());
                break;
        }
    }

    // Modifies the sprite colour
    private void Handle_Effects(State state)
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
        Handle_Effects(stateComponent.state);

        sprite.material = new Material(damageMaterial);
        yield return new WaitForSeconds(0.2f);
        sprite.material = new Material(defaultMaterial);

        runningDamageEffect = false;
        Handle_Effects(stateComponent.state);
    }
    #region Getters and Setters

    public bool CanTransitionState
    {
        get
        {
            return canTransitionState;
        }
        set
        {
            canTransitionState = value;
        }
    }
    #endregion
}
