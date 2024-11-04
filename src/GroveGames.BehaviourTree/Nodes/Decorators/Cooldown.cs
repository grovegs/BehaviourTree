using System;

using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Cooldown : Decorator
{
    private readonly NodeState _cooldownState;
    private readonly float _waitTime;
    private DateTime _lastExecutionTime;

    public Cooldown(Node child, float waitTime, NodeState cooldownState = NodeState.FAILURE) : base(child)
    {
        _waitTime = waitTime;
        _lastExecutionTime = DateTime.MinValue;
    }

    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        var currentTime = DateTime.Now;
        var timeSinceLastExecution = (currentTime - _lastExecutionTime).TotalSeconds;

        if (timeSinceLastExecution >= _waitTime)
        {
            _lastExecutionTime = currentTime;
            return child.Evaluate(blackboard, delta);
        }

        return _cooldownState;
    }
}
