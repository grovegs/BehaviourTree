namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class Abort : Decorator
{
    private readonly Func<bool> _condition;

    public Abort(Func<bool> condition, string? name = null) : base(name)
    {
        _condition = condition;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (_condition())
        {
            base.Abort();
            return _nodeState = NodeState.Success;
        }

        return _nodeState = base.Evaluate(deltaTime);
    }
}

public static partial class ParentExtensions
{
    public static IParent Abort(this IParent parent, Func<bool> condition, string? name = null)
    {
        var abort = new Abort(condition, name);
        parent.Attach(abort);
        return abort;
    }
}

