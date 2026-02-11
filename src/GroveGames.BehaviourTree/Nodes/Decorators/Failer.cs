namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Failer : Decorator
{
    public Failer(string? name = null) : base(name) { }

    public override NodeState Evaluate(float deltaTime)
    {
        var status = base.Evaluate(deltaTime);

        return status == NodeState.Running ? _nodeState = NodeState.Running : _nodeState = NodeState.Failure;
    }
}

public static partial class ParentExtensions
{
    public static IParent Failer(this IParent parent, string? name = null)
    {
        var failer = new Failer(name);
        parent.Attach(failer);
        return failer;
    }
}
