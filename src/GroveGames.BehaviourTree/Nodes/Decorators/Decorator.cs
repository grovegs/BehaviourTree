using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public class Decorator : Node
{
    protected readonly INode _child;

    public Decorator(INode parent, IBlackboard blackboard, INode child) : base(parent, blackboard)
    {
        _child = child;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        return _child.Evaluate(deltaTime);
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
