namespace GroveGames.BehaviourTree.Nodes.Decorators;

public abstract class Decorator : BehaviourNode, IParent
{
    private INode _child;

    public Decorator(string? name = null) : base(name)
    {
        _child = Empty;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        return _child.Evaluate(deltaTime); ;
    }

    public override void StartEvaluate()
    {
        _child.StartEvaluate();
    }

    public override void EndEvaluate()
    {
        _child.EndEvaluate();
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

        node.SetParent(this);
        _child = node;
        return this;
    }

    public IParent Attach(IChildTree tree)
    {
        tree.SetupTree(this);
        return this;
    }
}
