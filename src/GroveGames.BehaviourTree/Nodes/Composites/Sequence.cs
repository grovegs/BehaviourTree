namespace GroveGames.BehaviourTree.Nodes.Composites;

public sealed class Sequence : Composite
{
    private int _processingChildIndex;

    public Sequence(string? name = null) : base(name)
    {
        _processingChildIndex = 0;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (Children.Count == 0)
        {
            return _nodeState = NodeState.Success;
        }

        while (_processingChildIndex < Children.Count)
        {
            var child = Children[_processingChildIndex];
            var state = child.Evaluate(deltaTime);

            switch (state)
            {
                case NodeState.Running:
                    return _nodeState = NodeState.Running;

                case NodeState.Failure:
                    _processingChildIndex = 0;
                    return _nodeState = NodeState.Failure;

                case NodeState.Success:
                    _processingChildIndex++;
                    break;
            }
        }

        if (_processingChildIndex >= Children.Count)
        {
            Reset();
            return _nodeState = NodeState.Success;
        }

        return _nodeState = NodeState.Running;
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
    public static IParent Sequence(this IParent parent, string? name = null)
    {
        var sequence = new Sequence(name);
        parent.Attach(sequence);
        return sequence;
    }
}
