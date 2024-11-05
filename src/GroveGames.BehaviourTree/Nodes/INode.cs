using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public interface INode
{
    NodeState Evaluate(IBlackboard blackboard, float deltaTime);
    void BeforeEvaluate();
    void AfterEvaluate();
    void Reset();
    void Abort();
}