using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public sealed class Root : IParent
{
    private readonly IBlackboard _blackboard;
    private INode _child;

    public IBlackboard Blackboard => _blackboard;

    public Root(IBlackboard blackboard)
    {
        _blackboard = blackboard;
    }

    public NodeState Evaluate(float deltaTime)
    {
        return _child.Evaluate(deltaTime);
    }

    public void Abort()
    {
        _child.Abort();
    }

    public void Reset()
    {
        _child.Reset();
    }

    public IParent Attach(INode node)
    {
        _child = node;
        return this;
    }
}
