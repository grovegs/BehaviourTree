using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class Abort : Decorator
{
    private readonly Func<bool> _condition;

    public Abort(INode parent, INode child, Func<bool> condition) : base(parent, child)
    {
        _condition = condition;
    }

    public override NodeState Evaluate(IBlackboard blackboard, float deltaTime)
    {
        if (_condition())
        {
            _child.Abort();
            return NodeState.Failure;
        }

        return base.Evaluate(blackboard, deltaTime);
    }
}
