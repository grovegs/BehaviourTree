using System;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public class Sequence : Composite
{
    private int _currentChildIndex;

    public Sequence(Blackboard blackboard) : base(blackboard)
    {
    }


    public override NodeState Evaluate()
    {

        while (_currentChildIndex < childeren.Count)
        {
            var child = childeren[_currentChildIndex];

            child.BeforeEvaluate();
            var state = child.Evaluate();

            switch (state)
            {
                case NodeState.RUNNING:
                    return NodeState.RUNNING;

                case NodeState.FAILURE:
                    _currentChildIndex = 0;
                    return NodeState.FAILURE;

                case NodeState.SUCCESS:
                    _currentChildIndex++;
                    break;
            }

            child.AfterEvaluate();
        }

        return NodeState.SUCCESS;
    }
}
