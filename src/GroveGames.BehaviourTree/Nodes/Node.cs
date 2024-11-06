using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public abstract class Node : INode
{
    public static readonly INode Empty = new NullNode();

    private readonly IParent _parent;
    private readonly IBlackboard _blackboard;

    protected IParent Parent => _parent;
    public IBlackboard Blackboard => _blackboard;

    public Node(IParent parent, IBlackboard blackboard)
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
