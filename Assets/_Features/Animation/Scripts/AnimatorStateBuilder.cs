using NUnit.Framework;
using UnityEngine;

public class AnimatorStateBuilder
{
    private MotionComponent motionComponent;
    private StateComponent stateComponent;

    public AnimatorStateBuilder(MotionComponent motionComponent, StateComponent stateComponent)
    {
        this.motionComponent = motionComponent;
        this.stateComponent = stateComponent;
    }

    public class Builder
    {
        private MotionComponent motionComponent;
        private StateComponent stateComponent;

        public AnimatorStateBuilder Build(MotionComponent motionComponent, StateComponent stateComponent)
        {
            Assert.IsNotNull(motionComponent);
            return new AnimatorStateBuilder(motionComponent, stateComponent);
        }
    }


    /// <summary>
    /// Main responsibility of class.
    /// </summary>
    /// <returns></returns>
    public AnimatorState CreateAnimatorState()
    {
        var defaultDir = Vector2.zero;
        var defaultState = State.IDLE;

        var direction = motionComponent?.Dir ?? defaultDir;
        var lastFullDirection = motionComponent?.LastDir ?? defaultDir;
        var state = stateComponent?.State ?? defaultState;
        return new AnimatorState(direction, lastFullDirection, state);
    }
}
