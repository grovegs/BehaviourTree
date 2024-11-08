using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public sealed class Root : IRoot
{
    private readonly IBlackboard _blackboard;
    private INode _child;

    public IBlackboard Blackboard => _blackboard;

    public Root(IBlackboard blackboard)
    {
        _blackboard = blackboard;
        _child = Node.Empty;
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
        if (_child != Node.Empty)
        {
            throw new ChildAlreadyAttachedException();
        }

        _child = node;
        return this;
    }
}
