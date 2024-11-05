using System;

using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public sealed class Sequence : Composite
{
    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        if (processingChildIndex < children.Count)
        {
            var child = children[processingChildIndex];

            child.BeforeEvaluate();
            var state = child.Evaluate(blackboard, delta);

            switch (state)
            {
                case NodeState.RUNNING:
                    return NodeState.RUNNING;

                case NodeState.FAILURE:
                    processingChildIndex = 0;
                    return NodeState.FAILURE;

                case NodeState.SUCCESS:
                    processingChildIndex++;
                    return processingChildIndex == children.Count ? NodeState.SUCCESS : NodeState.RUNNING;
            }

            child.AfterEvaluate();
        }

        Reset();
        return NodeState.SUCCESS;
    }
}
