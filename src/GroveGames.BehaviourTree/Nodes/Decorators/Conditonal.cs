namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Conditonal : Decorator
{
    private readonly Func<bool> _condition;

    public Conditonal(IParent parent, Func<bool> condition) : base(parent)
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
        var conditional = new Conditonal(parent, condition);
        parent.Attach(conditional);
        return conditional;
    }
}
