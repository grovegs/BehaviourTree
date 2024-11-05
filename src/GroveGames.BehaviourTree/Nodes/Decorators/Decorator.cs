using System;

using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class Decorator : Node
{
    protected readonly Node child;

    public Decorator(Node child)
    {
        this.child = child;
    }

    public override void BeforeEvaluate()
    {
        child?.BeforeEvaluate();
    }

    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        return child != null ? child.Evaluate(blackboard, delta) : NodeState.FAILURE;
    }

    public override void AfterEvaluate()
    {
        child?.AfterEvaluate();
    }

    public override void Reset()
    {
        child?.Reset();
    }

    public override void Abort()
    {
        child?.Abort();
    }
}
