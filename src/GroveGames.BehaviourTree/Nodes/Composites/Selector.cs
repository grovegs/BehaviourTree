using System;

using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public class Selector : Composite
{
    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        while (processingChild < childeren.Count)
        {
            var child = childeren[processingChild];

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
        }

        if (processingChild >= childeren.Count)
        {
            processingChild = 0;
        }

        return NodeState.FAILURE;
    }
}
