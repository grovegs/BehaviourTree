using System;

using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Inverter : Decorator
{
    public Inverter(Node child) : base(child)
    {
    }

    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        var status = child.Evaluate(blackboard, delta);

        return status switch
        {
            NodeState.SUCCESS => NodeState.FAILURE,
            NodeState.RUNNING => NodeState.RUNNING,
            NodeState.FAILURE => NodeState.SUCCESS,
            _ => NodeState.FAILURE,
        };
    }
}
