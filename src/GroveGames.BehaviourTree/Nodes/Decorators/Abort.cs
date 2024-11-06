using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class Abort : Decorator
{
    private readonly Func<bool> _condition;

    public Abort(IParent parent, IBlackboard blackboard, Func<bool> condition) : base(parent, blackboard)
    {
        _condition = condition;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (_condition())
        {
            base.Abort();
            return NodeState.Success;
        }

        return base.Evaluate(deltaTime);
    }
}

public static partial class ParentExtensions
{
    public static void AttachAbort(this IParent parent, Func<bool> condition)
    {
        var abort = new Abort(parent, parent.Blackboard, condition);
        parent.Attach(abort);
    }
}

