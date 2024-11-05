using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Succeeder : Decorator
{
    public Succeeder(INode parent, INode child) : base(parent, child)
    {
    }

    public override NodeState Evaluate(IBlackboard blackboard, float deltaTime)
    {
        var status = base.Evaluate(blackboard, deltaTime);

        return status == NodeState.Running ? NodeState.Running : NodeState.Success;
    }
}
