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
        var children = Children;

        for (var i = 0; i < children.Count; i++)
        {
            var status = children[i].Evaluate(deltaTime);

            switch (status)
            {
                case NodeState.Success:
                    if (_policy == ParallelPolicy.AnySuccess)
                    {
                        AbortRunning(i);
                        return _nodeState = NodeState.Success;
                    }

                    break;

                case NodeState.Running:
                    allSuccess = false;
                    anyChildRunning = true;
                    break;

                default:
                    allSuccess = false;

                    if (_policy == ParallelPolicy.FirstFailure)
                    {
                        AbortRunning(i);
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

    private void AbortRunning(int except)
    {
        var children = Children;

        for (var i = 0; i < children.Count; i++)
        {
            if (i != except && children[i].State == NodeState.Running)
            {
                children[i].Abort();
            }
        }
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
