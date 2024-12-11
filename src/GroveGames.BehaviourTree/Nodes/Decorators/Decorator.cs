namespace GroveGames.BehaviourTree.Nodes.Decorators;

public abstract class Decorator : Node, IParent
{
    private INode _child;

    public Decorator(IParent parent) : base(parent)
    {
        _child = Empty;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        return _child.Evaluate(deltaTime);
    }

    public override void Reset()
    {
        _child.Reset();
    }

    public override void Abort()
    {
        _child.Abort();
    }

    public IParent Attach(INode node)
    {
        if (_child != Empty)
        {
            throw new ChildAlreadyAttachedException();
        }

        _child = node;
        return this;
    }

    public IParent Attach(IChildTree tree)
    {
        tree.SetupTree(this);
        return this;
    }
}
