using System;

using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class Interrupt : Decorator
{
    private readonly Func<bool> _shouldInterreupt;

    public Interrupt(Node child, Func<bool> interrupt) : base(child)
    {
        _shouldInterreupt = interrupt;
    }

    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        if (_shouldInterreupt())
        {
            child?.Interrupt();
            return NodeState.FAILURE;
        }

        return base.Evaluate(blackboard, delta);
    }
}
