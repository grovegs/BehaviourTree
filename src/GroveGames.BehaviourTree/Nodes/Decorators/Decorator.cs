using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public abstract class Decorator : Node, IParent
{
    private INode _child;

    public Decorator(IParent parent, IBlackboard blackboard) : base(parent, blackboard)
    {
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

    public IParent Attach(INode node)
    {
        _child = node;
        return this;
    }
}