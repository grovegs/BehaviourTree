using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class Decorator : Node
{
    protected readonly INode _child;

    public Decorator(INode parent, INode child) : base(parent)
    {
        _child = child;
    }

    public override void BeforeEvaluate()
    {
        _child?.BeforeEvaluate();
    }

    public override NodeState Evaluate(IBlackboard blackboard, float deltaTime)
    {
        return _child != null ? _child.Evaluate(blackboard, deltaTime) : NodeState.Failure;
    }

    public override void AfterEvaluate()
    {
        _child.AfterEvaluate();
    }

    public override void Reset()
    {
        _child.Reset();
    }

    public override void Abort()
    {
        _child.Abort();
    }
}
