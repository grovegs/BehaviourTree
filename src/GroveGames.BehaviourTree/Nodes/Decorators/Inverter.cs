using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Inverter : Decorator
{
    public Inverter(INode parent, INode child) : base(parent, child)
    {
    }

    public override NodeState Evaluate(IBlackboard blackboard, float deltaTime)
    {
        var status = _child.Evaluate(blackboard, deltaTime);

        return status switch
        {
            NodeState.Success => NodeState.Failure,
            NodeState.Running => NodeState.Running,
            NodeState.Failure => NodeState.Success,
            _ => NodeState.Failure,
        };
    }
}
