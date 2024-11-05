using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public abstract class Node : INode
{
    public static readonly INode Empty = new NullNode();

    private readonly INode _parent;

    public INode Parent => _parent;

    public Node(INode parent)
    {
        _parent = parent;
    }

    public virtual NodeState Evaluate(IBlackboard blackboard, float deltaTime)
    {
        return NodeState.Failure;
    }

    public virtual void BeforeEvaluate()
    {

    }

    public virtual void AfterEvaluate()
    {

    }

    public virtual void Reset()
    {

    }

    public virtual void Abort()
    {

    }
}
