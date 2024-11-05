using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public class Node
{
    protected Node parent;

    public void SetParent(Node parent)
    {
        this.parent = parent;
    }

    public virtual NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        return NodeState.FAILURE;
    }

    public virtual void BeforeEvaluate()
    {

    }

    public virtual void AfterEvaluate()
    {

    }

    public virtual void Reset()
    {

    }

    public virtual void Abort()
    {

    }
}
