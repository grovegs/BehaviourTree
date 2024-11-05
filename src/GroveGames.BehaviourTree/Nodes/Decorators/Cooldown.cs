using System;

using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Cooldown : Decorator
{
    private readonly float _waitTime;
    private float _remainingTime;

    public Cooldown(Node child, float waitTime) : base(child)
    {
        _waitTime = waitTime;
    }

    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        if (_remainingTime > 0f)
        {
            _remainingTime -= (float)delta;
            return NodeState.FAILURE;
        }

        _remainingTime = _waitTime;
        return base.Evaluate(blackboard, delta);
    }

    public override void Reset()
    {
        _remainingTime = 0f;
        base.Reset();
    }

    public override void Abort()
    {
        _remainingTime = 0f;
        base.Abort();
    }
}
