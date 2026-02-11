namespace GroveGames.BehaviourTree.Nodes.Composites;

public sealed class Parallel : Composite
{
    private readonly ParallelPolicy _policy;

    public Parallel(ParallelPolicy policy, string? name = null) : base(name)
    {
        _policy = policy;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        var allSuccess = true;
        var anyChildRunning = false;

        foreach (var child in Children)
        {
            var status = child.Evaluate(deltaTime);

            switch (status)
            {
                case NodeState.Success:
                    if (_policy == ParallelPolicy.AnySuccess)
                    {
                        return _nodeState = NodeState.Success;
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
                        return _nodeState = NodeState.Failure;
                    }
                    break;
            }
        }

        if (allSuccess && _policy == ParallelPolicy.AllSuccess)
        {
            return _nodeState = NodeState.Success;
        }

        return anyChildRunning ? _nodeState = NodeState.Running : _nodeState = NodeState.Failure;
    }
}

public static partial class ParentExtensions
{
    public static IParent Parallel(this IParent parent, ParallelPolicy policy, string? name = null)
    {
        var parallel = new Parallel(policy, name);
        parent.Attach(parallel);
        return parallel;
    }
}
