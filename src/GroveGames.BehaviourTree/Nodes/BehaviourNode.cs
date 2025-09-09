using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public abstract class BehaviourNode : INode
{
    public static readonly INode Empty = new EmptyBehaviourNode();

    private readonly IParent _parent;
    protected NodeState _nodeState;

    protected IParent Parent => _parent;
    public IBlackboard Blackboard => _parent.Blackboard;
    public NodeState State => _nodeState;

    public BehaviourNode(IParent parent)
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

    public virtual void StartEvaluate()
    {

    }

    public virtual void EndEvaluate()
    {

    }
}
