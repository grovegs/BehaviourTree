using System;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public class Sequence : Composite
{
    public override NodeState Evaluate(Blackboard blackboard, double delta)
    {

        while (processingChild < childeren.Count)
        {
            var child = childeren[processingChild];

            child.BeforeEvaluate();
            var state = child.Evaluate(blackboard, delta);

            switch (state)
            {
                case NodeState.RUNNING:
                    return NodeState.RUNNING;

                case NodeState.FAILURE:
                    processingChild = 0;
                    return NodeState.FAILURE;

                case NodeState.SUCCESS:
                    processingChild++;
                    break;
            }

            child.AfterEvaluate();
        }

        return NodeState.SUCCESS;
    }
}
