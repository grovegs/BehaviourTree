namespace GroveGames.BehaviourTree.Nodes.Composites;

public abstract class Composite : BehaviourNode, IParent
{
    private readonly List<INode> _children;

    protected IReadOnlyList<INode> Children => _children;

    public Composite()
    {
        _children = [];
    }

    public IParent Attach(INode node)
    {
        node.SetParent(this);
        _children.Add(node);
        return this;
    }

    public IParent Attach(IChildTree tree)
    {
        tree.SetupTree(this);
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

