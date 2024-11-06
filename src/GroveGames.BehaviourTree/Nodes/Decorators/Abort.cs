using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class Abort : Decorator
{
    private readonly Func<bool> _condition;

    public Abort(INode parent, IBlackboard blackboard, INode child, Func<bool> condition) : base(parent, blackboard, child)
    {
        _condition = condition;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (_condition())
        {
            _child.Abort();
            return NodeState.Success;
        }

        return base.Evaluate(deltaTime);
    }
}