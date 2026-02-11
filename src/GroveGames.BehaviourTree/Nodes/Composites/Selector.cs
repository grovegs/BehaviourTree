namespace GroveGames.BehaviourTree.Nodes.Composites;

public sealed class Selector : Composite
{
    private int _processingChildIndex;

    public Selector(string? name = null) : base(name)
    {
        _processingChildIndex = 0;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (Children.Count == 0)
        {
            return _nodeState = NodeState.Failure;
        }

        while (_processingChildIndex < Children.Count)
        {
            var child = Children[_processingChildIndex];
            var status = child.Evaluate(deltaTime);

            switch (status)
            {
                case NodeState.Running:
                    return _nodeState = NodeState.Running;

                case NodeState.Success:
                    _processingChildIndex = 0;
                    return _nodeState = NodeState.Success;

                case NodeState.Failure:
                    _processingChildIndex++;
                    break;
            }
        }

        Reset();
        return _nodeState = NodeState.Failure;
    }

    public override void Reset()
    {
        base.Reset();
        _processingChildIndex = 0;
    }

    public override void Abort()
    {
        if (_processingChildIndex < Children.Count)
        {
            Children[_processingChildIndex].Abort();
        }

        _processingChildIndex = 0;
    }
}

public static partial class ParentExtensions
{
    public static IParent Selector(this IParent parent, string? name = null)
    {
        var selector = new Selector(name);
        parent.Attach(selector);
        return selector;
    }
}
