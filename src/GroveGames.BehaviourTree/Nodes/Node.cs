using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public abstract class Node : INode
{
    private readonly IParent _parent;

    protected IParent Parent => _parent;
    public IBlackboard Blackboard => _parent.Blackboard;

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
