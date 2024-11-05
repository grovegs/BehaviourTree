using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree.Tree;

public abstract class BehaviourTree
{
    protected Node root;

    public abstract void SetupTree();

    public virtual void Tick(IBlackboard blackboard, double delta)
    {
        root?.BeforeEvaluate();
        root?.Evaluate(blackboard, delta);
        root?.AfterEvaluate();
    }

    public virtual void Reset()
    {
        root?.Reset();
    }

    public virtual void Abort()
    {
        root?.Abort();
    }
}
