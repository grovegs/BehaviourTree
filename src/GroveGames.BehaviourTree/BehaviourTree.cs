using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree;

public abstract class BehaviourTree : ITree
{
    private bool _isEnabled;
    private readonly IRoot _root;

    protected IRoot Root => _root;

    public BehaviourTree(IRoot root)
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
        OnEnable();
    }

    public void Disable()
    {
        _isEnabled = false;
        OnDisable();
    }

    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDisable()
    {
    }
}
