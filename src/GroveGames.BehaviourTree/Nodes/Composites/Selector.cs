using System;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public class Selector : Composite
{
    public Selector(Blackboard blackboard) : base(blackboard)
    {
    }


    public override NodeState Evaluate()
    {
        foreach (var child in childeren)
        {
            child.BeforeEvaluate();

            var state = child.Evaluate();

            switch (state)
            {
                case NodeState.SUCCESS:
                    return NodeState.SUCCESS;

                case NodeState.RUNNING:
                    return NodeState.RUNNING;
            }

            child.AfterEvaluate();
        }

        return NodeState.FAILURE;
    }
}
