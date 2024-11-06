using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Succeeder : Decorator
{
    public Succeeder(IParent parent, IBlackboard blackboard) : base(parent, blackboard)
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
    public static void AttachSucceeder(this IParent parent)
    {
        var succeeder = new Succeeder(parent, parent.Blackboard);
        parent.Attach(succeeder);
    }
}
