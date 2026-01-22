namespace GroveGames.BehaviourTree.Nodes;

public interface INode
{
    NodeState Evaluate(float deltaTime);
    void Reset();
    void Abort();
    void StartEvaluate();
    void EndEvaluate();
    void SetParent(IParent parent);
    NodeState State { get; }
}