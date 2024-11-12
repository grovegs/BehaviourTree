namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Inverter : Decorator
{
    public Inverter(IParent parent) : base(parent)
    {
    }

    public override NodeState Evaluate(float deltaTime)
    {
        var status = base.Evaluate(deltaTime);

        return _nodeState = status switch
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
    public static IParent Inverter(this IParent parent)
    {
        var inverter = new Inverter(parent);
        parent.Attach(inverter);
        return inverter;
    }
}

