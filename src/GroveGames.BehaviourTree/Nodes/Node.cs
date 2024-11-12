using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public abstract class Node : INode
{
    public static readonly INode Empty = new EmptyNode();

    private readonly IParent _parent;
    protected NodeState _nodeState;

    protected IParent Parent => _parent;
    public IBlackboard Blackboard => _parent.Blackboard;
    public NodeState State => _nodeState;

    public Node(IParent parent)
    {
        _parent = parent;
    }

    public virtual NodeState Evaluate(float deltaTime)
    {
        return NodeState.Failure;
    }

    public virtual void Reset()
    {

    }

    public virtual void Abort()
    {

    }
}
