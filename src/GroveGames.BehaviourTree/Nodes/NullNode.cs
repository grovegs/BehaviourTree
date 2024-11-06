using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public sealed class NullNode : INode, IParent
{
    public IBlackboard Blackboard => throw new NotSupportedException("Blackboard is not supported in NullNode.");

    public NodeState Evaluate(float deltaTime)
    {
        return NodeState.Failure;
    }

    public void Reset()
    {
    }

    public void Abort()
    {
    }

    public IParent Attach(INode node)
    {
        return this;
    }
}
