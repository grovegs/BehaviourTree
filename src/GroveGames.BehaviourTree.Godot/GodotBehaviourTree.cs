using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree;

public abstract partial class GodotBehaviourTree : Godot.Node, ITree
{
    private bool _isEnabled;
    private IRoot _root;
    protected IRoot Root => _root;

    public abstract void SetupTree();

    public void SetRoot(IRoot root)
    {
        _root = root;
    }

    public void Tick(float deltaTime)
    {
        if (!_isEnabled)
        {
            return;
        }

        _root.Evaluate(deltaTime);
    }

    public void Abort()
    {
        _root.Abort();
    }

    public void Disable()
    {
        _isEnabled = false;
        OnDisable();
    }

    public void Enable()
    {
        _isEnabled = true;
        OnEnable();
    }

    public void Reset()
    {
        _root.Reset();
    }

    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDisable()
    {
    }
}
