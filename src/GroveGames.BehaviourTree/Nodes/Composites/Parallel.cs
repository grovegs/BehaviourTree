using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public sealed class Parallel : Composite
{
    private readonly ParallelPolicy _policy;

    public Parallel(INode parent, ParallelPolicy policy) : base(parent)
    {
        _policy = policy;
    }

    public override NodeState Evaluate(IBlackboard blackboard, float deltaTime)
    {
        var allSuccess = true;
        var anyChildRunning = false;

        foreach (var child in Children)
        {
            child.BeforeEvaluate();
            var status = child.Evaluate(blackboard, deltaTime);

            switch (status)
            {
                case NodeState.Success:
                    if (_policy == ParallelPolicy.AnySuccess)
                    {
                        return NodeState.Success;
                    }

                    break;

                case NodeState.Running:
                    allSuccess = false;
                    anyChildRunning = true;
                    break;

                case NodeState.Failure:
                    allSuccess = false;

                    if (_policy == ParallelPolicy.FirstFailure)
                    {
                        return NodeState.Failure;
                    }
                    break;
            }

            child.AfterEvaluate();
        }

        if (allSuccess && _policy == ParallelPolicy.AllSuccess)
        {
            return NodeState.Success;
        }

        return anyChildRunning ? NodeState.Running : NodeState.Failure;
    }
}