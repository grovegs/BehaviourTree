using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree.Tree;

public abstract class BehaviourTree
{
    protected Node root;

    public abstract void SetupTree();

    public virtual void Tick(IBlackboard blackboard, double delta)
    {
        if (root != null)
        {
            root.BeforeEvaluate();
            root.Evaluate(blackboard, delta);
            root.AfterEvaluate();
        }
    }

    public virtual void Interrupt()
    {
        root.Interrupt();
    }
}
