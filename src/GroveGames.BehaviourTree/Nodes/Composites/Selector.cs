using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public sealed class Selector : Composite
{
    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        if (processingChildIndex < children.Count)
        {
            var child = children[processingChildIndex];

            child.BeforeEvaluate();

            var status = child.Evaluate(blackboard, delta);

            switch (status)
            {
                case NodeState.FAILURE:
                    processingChildIndex++;
                    return NodeState.RUNNING;

                case NodeState.RUNNING:
                    return NodeState.RUNNING;

                case NodeState.SUCCESS:
                    Reset();
                    return NodeState.SUCCESS;
            }

            child.AfterEvaluate();
        }

        Reset();
        return NodeState.FAILURE;
    }
}
