namespace GroveGames.BehaviourTree.Nodes.Composites;

public sealed class Selector : Composite
{
    private int _processingChildIndex;

    public Selector(IParent parent) : base(parent)
    {
        _processingChildIndex = 0;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (_processingChildIndex < Children.Count)
        {
            var child = Children[_processingChildIndex];
            var status = child.Evaluate(deltaTime);

            switch (status)
            {
                case NodeState.Failure:
                    child.EndEvaluate();
                    _processingChildIndex++;

                    if (_processingChildIndex < Children.Count)
                    {
                        Children[_processingChildIndex].StartEvaluate();
                    }

                    return _nodeState = NodeState.Running;

                case NodeState.Running:
                    return _nodeState = NodeState.Running;

                case NodeState.Success:
                    child.EndEvaluate();
                    Reset();
                    return _nodeState = NodeState.Success;
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
    public static IParent Selector(this IParent parent)
    {
        var selector = new Selector(parent);
        parent.Attach(selector);
        return selector;
    }
}
