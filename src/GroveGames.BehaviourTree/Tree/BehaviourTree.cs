using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree.Tree;

public abstract class BehaviourTree
{
    private readonly INode _root;

    public BehaviourTree(INode root)
    {
        _root = root;
    }

    public abstract void SetupTree();

    public virtual void Tick(IBlackboard blackboard, float deltaTime)
    {
        _root.BeforeEvaluate();
        _root.Evaluate(blackboard, deltaTime);
        _root.AfterEvaluate();
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
