using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Inverter : Decorator
{
    public Inverter(IParent parent, IBlackboard blackboard) : base(parent, blackboard)
    {
    }

    public override NodeState Evaluate(float deltaTime)
    {
        var status = base.Evaluate(deltaTime);

        return status switch
        {
            NodeState.Success => NodeState.Failure,
            NodeState.Running => NodeState.Running,
            NodeState.Failure => NodeState.Success,
            _ => NodeState.Failure,
        };
    }
}

public static partial class ParentExtensions
{
    public static void AttachInverter(this IParent parent)
    {
        var inverter = new Inverter(parent, parent.Blackboard);
        parent.Attach(inverter);
    }
}

