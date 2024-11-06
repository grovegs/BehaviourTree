using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree;

public abstract class Tree
{
    private readonly Root _root;

    public Tree(Root root)
    {
        _root = root;
    }

    public abstract void SetupTree();

    public virtual void Tick(float deltaTime)
    {
        _root.Evaluate(deltaTime);
    }

    public virtual void Reset()
    {
        _root.Reset();
    }

    public virtual void Abort()
    {
        _root.Abort();
    }
}
