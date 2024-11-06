using System;

using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public sealed class Root : Node, IParent
{
    private INode _child;

    public Root(IBlackboard blackboard) : base(Empty, blackboard)
    {
    }

    public override NodeState Evaluate(float deltaTime)
    {
        return _child.Evaluate(deltaTime);
    }

    public override void Abort()
    {
        _child.Abort();
    }

    public override void Reset()
    {
        _child.Reset();
    }

    public IParent Attach(INode node)
    {
        _child = node;
        return this;
    }
}
