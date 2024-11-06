using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public abstract class Node : INode
{
    public static readonly INode Empty = new NullNode();

    private readonly INode _parent;
    private readonly IBlackboard _blackboard;

    protected INode Parent => _parent;
    protected IBlackboard Blackboard => _blackboard;

    public Node(INode parent, IBlackboard blackboard)
    {
        _parent = parent;
        _blackboard = blackboard;
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
