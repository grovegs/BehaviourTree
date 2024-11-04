using System;

using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Failer : Decorator
{
    public Failer(Node child) : base(child)
    {
    }

    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        var status = base.Evaluate(blackboard, delta);

        return status == NodeState.RUNNING ? NodeState.RUNNING : NodeState.FAILURE;
    }
}
