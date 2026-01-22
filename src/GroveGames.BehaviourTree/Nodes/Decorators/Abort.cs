namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class Abort : Decorator
{
    private readonly Func<bool> _condition;

    public Abort(Func<bool> condition)
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
    public static IParent Abort(this IParent parent, Func<bool> condition)
    {
        var abort = new Abort(condition);
        parent.Attach(abort);
        return abort;
    }
}

