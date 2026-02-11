namespace GroveGames.BehaviourTree.Nodes;

public interface INode
{
    public NodeState Evaluate(float deltaTime);
    public void Reset();
    public void Abort();
    public void StartEvaluate();
    public void EndEvaluate();
    public void SetParent(IParent parent);
    public void SetName(string name);
    public NodeState State { get; }
    public string Name { get; }
}
