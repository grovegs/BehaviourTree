namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Succeeder : Decorator
{
    public Succeeder(IParent parent) : base(parent)
    {
    }

    public override NodeState Evaluate(float deltaTime)
    {
        var status = base.Evaluate(deltaTime);

        return status == NodeState.Running ? NodeState.Running : NodeState.Success;
    }
}

public static partial class ParentExtensions
{
    public static void Succeeder(this IParent parent)
    {
        var succeeder = new Succeeder(parent);
        parent.Attach(succeeder);
    }
}
