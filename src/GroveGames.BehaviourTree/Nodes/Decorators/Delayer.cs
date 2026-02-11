namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Delayer : Decorator
{
    private readonly float _waitTime;
    private float _interval;

    public Delayer(float waitTime, string? name = null) : base(name)
    {
        _waitTime = waitTime;
        _interval = 0f;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        _interval += deltaTime;

        if (_interval >= _waitTime)
        {
            _interval = 0f;
            return _nodeState = base.Evaluate(deltaTime);
        }

        return _nodeState = NodeState.Running;

    }

    public override void Abort()
    {
        base.Abort();
        _interval = 0f;
    }

    public override void Reset()
    {
        base.Reset();
        _interval = 0f;
    }
}

public static partial class ParentExtensions
{
    public static IParent Delayer(this IParent parent, float waitTime, string? name = null)
    {
        var delayer = new Delayer(waitTime, name);
        parent.Attach(delayer);
        return delayer;
    }
}
