namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Failer : Decorator
{
    public override NodeState Evaluate(float deltaTime)
    {
        var status = base.Evaluate(deltaTime);

        return status == NodeState.Running ? _nodeState = NodeState.Running : _nodeState = NodeState.Failure;
    }
}

public static partial class ParentExtensions
{
    public static IParent Failer(this IParent parent)
    {
        var failer = new Failer();
        parent.Attach(failer);
        return failer;
    }
}
