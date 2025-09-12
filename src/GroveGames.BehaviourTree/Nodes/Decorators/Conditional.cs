namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Conditional : Decorator
{
    private readonly Func<bool> _condition;

    public Conditional(IParent parent, Func<bool> condition) : base(parent)
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
    public static IParent Conditional(this IParent parent, Func<bool> condition)
    {
        var conditional = new Conditional(parent, condition);
        parent.Attach(conditional);
        return conditional;
    }
}
