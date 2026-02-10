namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Repeater : Decorator
{
    private readonly RepeatMode _repeatMode;
    private readonly int _maxCount;
    private int _currentCount;

    public Repeater(RepeatMode repeatMode, int maxCount = -1)
    {
        _repeatMode = repeatMode;
        _maxCount = maxCount;
        _currentCount = 0;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (_repeatMode == RepeatMode.FixedCount && _currentCount >= _maxCount)
        {
            _currentCount = 0;
            return _nodeState = NodeState.Success;
        }

        var childStatus = base.Evaluate(deltaTime);

        switch (_repeatMode)
        {
            case RepeatMode.FixedCount:
                if (childStatus == NodeState.Success || childStatus == NodeState.Failure)
                {
                    _currentCount++;
                    return _nodeState = NodeState.Running;
                }
                break;

            case RepeatMode.UntilSuccess:
                if (childStatus == NodeState.Success)
                {
                    return _nodeState = NodeState.Success;
                }
                return _nodeState = NodeState.Running;

            case RepeatMode.UntilFailure:
                if (childStatus == NodeState.Failure)
                {
                    return _nodeState = NodeState.Failure;
                }
                return _nodeState = NodeState.Running;

            case RepeatMode.Infinite:
                return _nodeState = NodeState.Running;
        }

        return childStatus == NodeState.Running ? _nodeState = NodeState.Running : _nodeState = NodeState.Success;
    }

    public override void Reset()
    {
        base.Reset();
        _currentCount = 0;
    }

    public override void Abort()
    {
        base.Abort();
        _currentCount = 0;
    }
}

public static partial class ParentExtensions
{
    public static IParent Repeater(this IParent parent, RepeatMode repeatMode)
    {
        var repeater = new Repeater(repeatMode);
        parent.Attach(repeater);
        return repeater;
    }
}
