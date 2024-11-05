namespace GroveGames.BehaviourTree.Nodes.Composites;

public class Composite : Node
{
    private readonly List<INode> _children;

    protected IReadOnlyList<INode> Children => _children;

    public Composite(INode parent) : base(parent)
    {
        _children = [];
    }

    public Composite AddChild(INode child)
    {
        _children.Add(child);

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
