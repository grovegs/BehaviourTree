using System;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public class Sequence : Composite
{
    private int _successIndex;

    public Sequence(Blackboard blackboard) : base(blackboard)
    {
    }


    public override NodeState Evaluate()
    {

        while (_successIndex < childeren.Count)
        {
            var child = childeren[_successIndex];

            child.BeforeEvaluate();
            var state = child.Evaluate();

            switch (state)
            {
                case NodeState.RUNNING:
                    return NodeState.RUNNING;

                case NodeState.FAILURE:
                    _successIndex = 0;
                    return NodeState.FAILURE;

                case NodeState.SUCCESS:
                    _successIndex++;
                    break;
            }

            child.AfterEvaluate();
        }

        return NodeState.SUCCESS;
    }
}
