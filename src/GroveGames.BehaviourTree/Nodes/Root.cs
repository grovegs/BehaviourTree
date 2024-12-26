using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public sealed class Root : IRoot
{
    private readonly IBlackboard _blackboard;
    private INode _child;
    private NodeState _nodeState;

    public IBlackboard Blackboard => _blackboard;
    public NodeState State => _nodeState;

    public Root(IBlackboard blackboard)
    {
        _blackboard = blackboard;
        _child = Node.Empty;
    }

    public NodeState Evaluate(float deltaTime)
    {
        return _nodeState = _child.Evaluate(deltaTime);
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

    public IParent Attach(IChildTree tree)
    {
        tree.SetupTree(this);
        return this;
    }

    public void OnEnter()
    {

    }

    public void OnExit()
    {

    }
}
