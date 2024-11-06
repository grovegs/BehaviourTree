using System;

using GroveGames.BehaviourTree.Nodes.Composites;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

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
        else
        {
            _aborted = false;
        }

        return base.Evaluate(deltaTime);
    }
}

public static partial class ParentExtensions
{
    public static IParent AborOnce(this IParent parent, Func<bool> condition)
    {
        var abort = new AbortOnce(parent, condition);
        parent.Attach(abort);
        return abort;
    }
}