namespace GroveGames.BehaviourTree.Nodes;

public sealed class NullNode : INode
{
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
}
