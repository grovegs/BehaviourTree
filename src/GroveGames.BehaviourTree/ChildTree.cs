using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree;

public abstract class ChildTree : IChildTree
{
    protected IParent Parent => _parent;

    public NodeState State => _nodeState;

    protected INode _head;
    protected NodeState _nodeState;
    private readonly IParent _parent;

    public ChildTree(IParent parent)
    {
        _parent = parent;
        _head = Node.Empty;
    }

    public abstract void SetupTree();

    public NodeState Evaluate(float deltaTime)
    {
        return _nodeState = _head.Evaluate(deltaTime);
    }

    public void Reset()
    {
        _head.Reset();
    }

    public void Abort()
    {
        _head.Abort();
    }
}
