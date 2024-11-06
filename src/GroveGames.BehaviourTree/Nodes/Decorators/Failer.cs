using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Failer : Decorator
{
    public Failer(INode parent, IBlackboard blackboard, INode child) : base(parent, blackboard, child)
    {
    }

    public override NodeState Evaluate(float deltaTime)
    {
        var status = base.Evaluate(deltaTime);

        return status == NodeState.Running ? NodeState.Running : NodeState.Failure;
    }
}
