namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class Reset : Decorator
{
    private readonly Func<bool> _condition;

    public Reset(Func<bool> condition, string? name = null) : base(name)
    {
        _condition = condition;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (_condition())
        {
            base.Reset();
            return _nodeState = NodeState.Success;
        }

        return _nodeState = base.Evaluate(deltaTime);
    }
}

public static partial class ParentExtensions
{
    public static IParent Reset(this IParent parent, Func<bool> condition, string? name = null)
    {
        var reset = new Reset(condition, name);
        parent.Attach(reset);
        return reset;
    }
}
