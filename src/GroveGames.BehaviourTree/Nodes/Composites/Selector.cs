using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public sealed class Selector : Composite
{
    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        var child = childeren[processingChild % childeren.Count];

        child.BeforeEvaluate();

        var status = child.Evaluate(blackboard, delta);

        switch (status)
        {
            case NodeState.FAILURE:
                processingChild++;
                break;

            case NodeState.RUNNING:
                return NodeState.RUNNING;

            case NodeState.SUCCESS:
                processingChild = 0;
                return NodeState.SUCCESS;
        }

        child.AfterEvaluate();

        return NodeState.FAILURE;
    }
}
