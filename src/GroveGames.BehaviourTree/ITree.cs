namespace GroveGames.BehaviourTree;

public interface ITree
{
    public void SetupTree();
    public void Reset();
    public void Abort();
    public void Enable();
    public void Disable();
    public void Tick(float deltaTime);
}
