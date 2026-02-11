namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Succeeder : Decorator
{
    public Succeeder(string? name = null) : base(name) { }

    public override NodeState Evaluate(float deltaTime)
    {
        var status = base.Evaluate(deltaTime);

        return status == NodeState.Running ? _nodeState = NodeState.Running : _nodeState = NodeState.Success;
    }
}

public static partial class ParentExtensions
{
    public static IParent Succeeder(this IParent parent, string? name = null)
    {
        var succeeder = new Succeeder(name);
        parent.Attach(succeeder);
        return succeeder;
    }
}
