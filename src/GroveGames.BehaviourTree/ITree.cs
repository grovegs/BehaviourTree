namespace GroveGames.BehaviourTree;

public interface ITree
{
    void SetupTree();
    void Reset();
    void Abort();
    void Enable();
    void Disable();
    void Tick(float deltaTime);
}
