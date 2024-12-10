using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree;

public abstract class Tree : ITree
{
    private bool _isEnabled;
    private readonly IRoot _root;
    public IRoot Root => _root;

    public Tree(IRoot root)
    {
        _root = root;
        _isEnabled = false;
    }

    public abstract void SetupTree();

    public virtual void Tick(float deltaTime)
    {
        if (!_isEnabled)
        {
            return;
        }

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

    public void Enable()
    {
        _isEnabled = true;
    }

    public void Disable()
    {
        _isEnabled = false;
    }

    void ITree.SetupTree()
    {
        throw new NotImplementedException();
    }

    void ITree.Reset()
    {
        throw new NotImplementedException();
    }

    void ITree.Abort()
    {
        throw new NotImplementedException();
    }

    void ITree.Enable()
    {
        throw new NotImplementedException();
    }

    void ITree.Disable()
    {
        throw new NotImplementedException();
    }

    void ITree.Tick(float deltaTime)
    {
        throw new NotImplementedException();
    }
}
