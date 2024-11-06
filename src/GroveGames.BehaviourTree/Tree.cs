using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree;

public abstract class Tree
{
    private readonly INode _root;

    public Tree(INode root)
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
