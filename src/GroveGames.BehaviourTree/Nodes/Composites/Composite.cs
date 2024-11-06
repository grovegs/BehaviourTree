using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public abstract class Composite : Node, IParent
{
    private readonly List<INode> _children;

    protected IReadOnlyList<INode> Children => _children;

    public Composite(IParent parent, IBlackboard blackboard) : base(parent, blackboard)
    {
        _children = [];
    }

    public IParent Attach(INode node)
    {
        _children.Add(node);
        return this;
    }

    public override void Reset()
    {
        foreach (var child in _children)
        {
            child.Reset();
        }
    }
}

