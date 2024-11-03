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
        return child.Evaluate(blackboard, delta);
    }

    public override void AfterEvaluate()
    {
        child?.AfterEvaluate();
    }

    public override void Interrupt()
    {
        child?.Interrupt();
    }
}
