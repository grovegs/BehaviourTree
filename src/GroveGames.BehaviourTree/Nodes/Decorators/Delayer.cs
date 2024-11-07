namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class Delayer : Decorator
{
    private readonly float _waitTime;
    private float _interval;

    public Delayer(IParent parent, float waitTime) : base(parent)
    {
        _waitTime = waitTime;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        _interval += deltaTime;

        if (_interval >= _waitTime)
        {
            _interval = 0f;
            return base.Evaluate(deltaTime);
        }
        else
        {
            return NodeState.Running;
        }
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
    public static IParent Delayer(this IParent parent, float waitTime)
    {
        var cooldown = new Delayer(parent, waitTime);
        parent.Attach(cooldown);
        return cooldown;
    }
}
