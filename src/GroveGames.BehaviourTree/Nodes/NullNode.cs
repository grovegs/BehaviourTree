using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public sealed class NullNode : INode
{
    public NodeState Evaluate(IBlackboard blackboard, float deltaTime)
    {
        return NodeState.Failure;
    }

    public void AfterEvaluate()
    {
    }

    public void BeforeEvaluate()
    {
    }

    public void Reset()
    {
    }

    public void Abort()
    {
    }
}
