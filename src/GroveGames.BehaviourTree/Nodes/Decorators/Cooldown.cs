using System;

using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Cooldown : Decorator
{
    private readonly float _waitTime;
    private float _interval;

    public Cooldown(Node child, float waitTime) : base(child)
    {
        _waitTime = waitTime;
    }

    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        _interval += (float)delta;

        if (_interval >= _waitTime)
        {
            _interval = 0f;
            return child != null ? child.Evaluate(blackboard, delta) : NodeState.FAILURE;
        }

        return NodeState.FAILURE;
    }

    public override void Interrupt()
    {
        _interval = 0f;
        base.Interrupt();
    }
}
