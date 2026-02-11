namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class SuccessOnce : Decorator
{
    private bool _hasSucceeded;

    public SuccessOnce(string? name = null) : base(name)
    {
        _hasSucceeded = false;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (_hasSucceeded)
        {
            return _nodeState = NodeState.Failure;
        }

        _nodeState = base.Evaluate(deltaTime);

        if (_nodeState == NodeState.Success)
        {
            _hasSucceeded = true;
        }

        return _nodeState;
    }

    public override void Reset()
    {
        base.Reset();
        _hasSucceeded = false;
    }

    public override void Abort()
    {
        base.Abort();
        _hasSucceeded = false;
    }
}

