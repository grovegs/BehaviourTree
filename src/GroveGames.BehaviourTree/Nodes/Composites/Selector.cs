using System;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public class Selector : Composite
{
    private int _currentChildIndex;

    public Selector(Blackboard blackboard) : base(blackboard)
    {
    }


    public override NodeState Evaluate()
    {
        while (_currentChildIndex < childeren.Count)
        {
            var child = childeren[_currentChildIndex];

            child.BeforeEvaluate();

            var status = child.Evaluate();

            switch (status)
            {
                case NodeState.FAILURE:
                    _currentChildIndex++;
                    break;

                case NodeState.RUNNING:
                    return NodeState.RUNNING;

                case NodeState.SUCCESS:
                    _currentChildIndex = 0;
                    return NodeState.SUCCESS;
            }

            child.AfterEvaluate();
        }

        return NodeState.FAILURE;
    }
}
