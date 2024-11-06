namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Cooldown : Decorator
{
    private readonly float _waitTime;
    private float _remainingTime;

    public Cooldown(IParent parent, float waitTime) : base(parent)
    {
        _waitTime = waitTime;
        _remainingTime = 0;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (_remainingTime > 0f)
        {
            _remainingTime -= deltaTime;
            return NodeState.Failure;
        }

        _remainingTime = _waitTime;
        return base.Evaluate(deltaTime);
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
    public static IParent Cooldown(this IParent parent, float waitTime)
    {
        var cooldown = new Cooldown(parent, waitTime);
        parent.Attach(cooldown);
        return cooldown;
    }
}
