using System;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public class Selector : Composite
{
    public Selector(Blackboard blackboard) : base(blackboard)
    {
    }


    public override NodeState Evaluate()
    {
        while (processingChild < childeren.Count)
        {
            var child = childeren[processingChild];

            child.BeforeEvaluate();

            var status = child.Evaluate();

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
