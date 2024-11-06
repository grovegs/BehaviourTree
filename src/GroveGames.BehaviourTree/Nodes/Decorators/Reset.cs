namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class Reset : Decorator
{
    private readonly Func<bool> _condition;

    public Reset(IParent parent, Func<bool> condition) : base(parent)
    {
        _condition = condition;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (_condition())
        {
            base.Reset();
            return NodeState.Success;
        }

        return base.Evaluate(deltaTime);
    }
}

public static partial class ParentExtensions
{
    public static void Reset(this IParent parent, Func<bool> condition)
    {
        var reset = new Reset(parent, condition);
        parent.Attach(reset);
    }
}
