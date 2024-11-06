using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class Reset : Decorator
{
    private readonly Func<bool> _condition;

    public Reset(IParent parent, IBlackboard blackboard, Func<bool> condition) : base(parent, blackboard)
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
    public static void AttachReset(this IParent parent, Func<bool> condition)
    {
        var reset = new Reset(parent, parent.Blackboard, condition);
        parent.Attach(reset);
    }
}
