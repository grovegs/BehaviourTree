namespace GroveGames.BehaviourTree.Nodes;

public interface INode
{
    NodeState Evaluate(float deltaTime);
    void Reset();
    void Abort();
    void OnEnter();
    void OnExit();
    NodeState State { get; }
}