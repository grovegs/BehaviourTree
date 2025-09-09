namespace GroveGames.BehaviourTree.Nodes;

public sealed class EmptyBehaviourNode : INode
{
    public NodeState State => NodeState.Failure;

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

    public void StartEvaluate()
    {

    }

    public void EndEvaluate()
    {

    }
}
