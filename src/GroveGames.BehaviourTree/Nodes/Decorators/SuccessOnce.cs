namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class SuccessOnce : Decorator
{
    private bool _hasSucceeded;

    public SuccessOnce(IParent parent) : base(parent)
    {
        _hasSucceeded = false;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (_hasSucceeded)
        {
            return NodeState.Failure;
        }

        var result = base.Evaluate(deltaTime);

        if (result == NodeState.Success)
        {
            _hasSucceeded = true;
        }

        return result;
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

