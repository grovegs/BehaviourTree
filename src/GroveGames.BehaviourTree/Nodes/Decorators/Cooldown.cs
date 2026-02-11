namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Cooldown : Decorator
{
    private readonly float _waitTime;
    private float _remainingTime;

    public Cooldown(float waitTime, string? name = null) : base(name)
    {
        _waitTime = waitTime;
        _remainingTime = 0;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        var remainingTime = _remainingTime - deltaTime;

        if (remainingTime > 0f)
        {
            _remainingTime = remainingTime;
            return _nodeState = NodeState.Failure;
        }

        _remainingTime = _waitTime;
        return _nodeState = base.Evaluate(deltaTime);
    }

    public override void Reset()
    {
        base.Reset();
        _remainingTime = 0f;
    }

    public override void Abort()
    {
        base.Abort();
        _remainingTime = 0f;
    }
}

public static partial class ParentExtensions
{
    public static IParent Cooldown(this IParent parent, float waitTime, string? name = null)
    {
        var cooldown = new Cooldown(waitTime, name);
        parent.Attach(cooldown);
        return cooldown;
    }
}
