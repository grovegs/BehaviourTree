using System;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public class Sequence : Composite
{
    public Sequence(Blackboard blackboard) : base(blackboard)
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
                case NodeState.RUNNING:
                    return NodeState.RUNNING;

                case NodeState.FAILURE:
                    return NodeState.FAILURE;
            }

            child.AfterEvaluate();
        }

        return NodeState.SUCCESS;
    }
}
