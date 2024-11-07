namespace GroveGames.BehaviourTree.Nodes;

public sealed class EmptyNode : INode
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
