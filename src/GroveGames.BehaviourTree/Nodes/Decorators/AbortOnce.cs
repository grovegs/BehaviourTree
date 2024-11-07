namespace GroveGames.BehaviourTree.Nodes.Decorators;

[Obsolete("AbortOnce is obsolete and may be removed in future versions. Use ExecuteOnce and Abort instead.")]
public class AbortOnce : Decorator
{
    private readonly Func<bool> _condition;
    private bool _aborted;

    public AbortOnce(IParent parent, Func<bool> condition) : base(parent)
    {
        _condition = condition;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (_condition() && !_aborted)
        {
            _aborted = true;
            base.Abort();
            return NodeState.Success;
        }
        else if (!_condition())
        {
            _aborted = false;
        }

        return base.Evaluate(deltaTime);
    }
}

public static partial class ParentExtensions
{
    [Obsolete("AbortOnce is obsolete and may be removed in future versions. Use ExecuteOnce and Abort instead.")]
    public static IParent AbortOnce(this IParent parent, Func<bool> condition)
    {
        var abort = new AbortOnce(parent, condition);
        parent.Attach(abort);
        return abort;
    }
}