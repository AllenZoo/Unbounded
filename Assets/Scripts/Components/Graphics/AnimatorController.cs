using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Animator))]
public class AnimatorController : MonoBehaviour
{
    [NotNull]
    [SerializeField] private LocalEventHandler localEventHandler;

    // For changing colour of the sprite (giving it a tint) based on state.
    [SerializeField] private SpriteRenderer sprite;

    // For making the sprite flash white. 
    [SerializeField] private Material damageMaterial;
    [SerializeField] private Material defaultMaterial;
    private bool runningDamageEffect = false;

    #region Animator Parameters
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

    #endregion

    // Components
    private Animator animator;

    // State in this case refers to animation states and not entity states.
    private bool canTransitionState = true;

    // Information about entity state.
    private State curState;
    // private State lastNonCCState;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        Assert.IsNotNull(sprite, "SpriteRenderer null in animation controller for object: " + gameObject);

        if (defaultMaterial == null)
        {
            defaultMaterial = sprite.sharedMaterial;

            /* Unmerged change from project 'Assembly-CSharp.Player'
            Before:
                    }

                    Assert.IsNotNull(damageMaterial, "Need material for being damaged");
            After:
                    }

                    Assert.IsNotNull(damageMaterial, "Need material for being damaged");
            */
        }

        Assert.IsNotNull(damageMaterial, "Need material for being damaged");

        localEventHandler = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);
    }

    private void Start()
    {
        Debug.Assert(animator != null, "Animator null in animation controller for object: " + gameObject);

        LocalEventBinding<OnMotionChangeEvent> motionEventBinding = new LocalEventBinding<OnMotionChangeEvent>(Motion_OnMotionChange);
        localEventHandler.Register<OnMotionChangeEvent>(motionEventBinding);

        LocalEventBinding<OnStateChangeEvent> stateEventBinding = new LocalEventBinding<OnStateChangeEvent>(HandleOnStateChange);
        localEventHandler.Register<OnStateChangeEvent>(stateEventBinding);

        LocalEventBinding<OnDamagedEvent> damagedEventBinding = new LocalEventBinding<OnDamagedEvent>(HandleOnDamagedEvent);
        localEventHandler.Register<OnDamagedEvent>(damagedEventBinding);


        // TODO: take a look at this.
        // Check Parameters are present in animator controller
        // Debug.Assert(animator.parameters.Length >= NUMBER_OF_PARAMETERS,
        //    "Animator parameters not set up correctly for object: " + gameObject);

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
    public void SetState(State state)
    {
        this.curState = state;
        Handle_Animation(state);
        Handle_Effects(state);
    }

    private void Motion_OnMotionChange(OnMotionChangeEvent e)
    {
        SetMovementParameters(e.newDir, e.lastDir);
    }

    private void HandleOnStateChange(OnStateChangeEvent e)
    {
        // Do it in this order to ensure that we animation transition State.DEAD.
        SetState(e.newState);
        switch (e.newState)
        {
            case State.DEAD:
                CanTransitionState = false;
                break;
            default:
                CanTransitionState = true;
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
        if (!canTransitionState)
        {
            return;
        }

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
        Handle_Effects(curState);

        sprite.material = damageMaterial;
        yield return new WaitForSeconds(0.2f);
        sprite.material = defaultMaterial;

        runningDamageEffect = false;
        Handle_Effects(curState);
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
