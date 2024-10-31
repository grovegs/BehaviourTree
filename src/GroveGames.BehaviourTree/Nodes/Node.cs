using System;

namespace GroveGames.BehaviourTree.Nodes;

public class Node
{
    protected readonly Blackboard blackboard;
    protected Node parent;

    public Node(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }

    public void SetParent(Node parent)
    {
        this.parent = parent;
    }

    public virtual NodeState Evaluate()
    {
        return NodeState.FAILURE;
    }

    public virtual void BeforeEvaluate()
    {

    }

    public virtual void AfterEvaluate()
    {

    }

    public virtual void Interrupt()
    {

    }
}
