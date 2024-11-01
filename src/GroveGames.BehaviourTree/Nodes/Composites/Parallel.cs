using System;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public enum ParallelPolicy
{
    ANY_SUCCESS,
    ALL_SUCCESS,
    FIRST_FAILURE
}

public class Parallel : Composite
{
    private readonly ParallelPolicy _policy;

    public Parallel(ParallelPolicy policy) : base()
    {
        _policy = policy;
    }

    public override NodeState Evaluate(Blackboard blackboard, double delta)
    {
        var allSuccess = true;
        var anyChildRunning = false;

        foreach (var child in childeren)
        {
            child.BeforeEvaluate();
            var status = child.Evaluate(blackboard, delta);

            switch (status)
            {
                case NodeState.SUCCESS:
                    if (_policy == ParallelPolicy.ANY_SUCCESS)
                    {
                        return NodeState.SUCCESS;
                    }

                    break;

                case NodeState.RUNNING:
                    allSuccess = false;
                    anyChildRunning = true;
                    break;

                case NodeState.FAILURE:
                    allSuccess = false;

                    if (_policy == ParallelPolicy.FIRST_FAILURE)
                    {
                        return NodeState.FAILURE;
                    }
                    break;
            }

            child.AfterEvaluate();
        }

        if (allSuccess && _policy == ParallelPolicy.ALL_SUCCESS)
        {
            return NodeState.SUCCESS;
        }

        return anyChildRunning ? NodeState.RUNNING : NodeState.FAILURE;
    }
}