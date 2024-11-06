using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class Reset : Decorator
{
    private readonly Func<bool> _condition;

    public Reset(INode parent, IBlackboard blackboard, INode child, Func<bool> condition) : base(parent, blackboard, child)
    {
        _condition = condition;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (_condition())
        {
            _child.Reset();
            return NodeState.Success;
        }

        return base.Evaluate(deltaTime);
    }
}
