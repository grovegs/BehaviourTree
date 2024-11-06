using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Failer : Decorator
{
    public Failer(IParent parent, IBlackboard blackboard) : base(parent, blackboard)
    {
    }

    public override NodeState Evaluate(float deltaTime)
    {
        var status = base.Evaluate(deltaTime);

        return status == NodeState.Running ? NodeState.Running : NodeState.Failure;
    }
}

public static partial class ParentExtensions
{
    public static void AttachFailer(this IParent parent)
    {
        var failer = new Failer(parent, parent.Blackboard);
        parent.Attach(failer);
    }
}
