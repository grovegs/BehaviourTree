namespace GroveGames.BehaviourTree.Nodes.Composites;

public abstract class Composite : BehaviourNode, IParent
{
    private readonly List<INode> _children;

    protected IReadOnlyList<INode> Children => _children;

    public Composite(string? name = null) : base(name)
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
        for (var i = 0; i < _children.Count; i++)
        {
            _children[i].Reset();
        }
    }

    public override void Abort()
    {
        for (var i = 0; i < _children.Count; i++)
        {
            _children[i].Abort();
        }
    }
}

