namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Conditional : Decorator
{
    private readonly Func<bool> _condition;

    public Conditional(Func<bool> condition, string? name = null) : base(name)
    {
        _condition = condition;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        return _condition() ? _nodeState = base.Evaluate(deltaTime) : _nodeState = NodeState.Failure;
    }
}

public static partial class ParentExtensions
{
    public static IParent Conditional(this IParent parent, Func<bool> condition, string? name = null)
    {
        var conditional = new Conditional(condition, name);
        parent.Attach(conditional);
        return conditional;
    }
}
