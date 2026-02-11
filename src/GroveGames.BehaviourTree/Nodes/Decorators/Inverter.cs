namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Inverter : Decorator
{
    public Inverter(string? name = null) : base(name) { }

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
    public static IParent Inverter(this IParent parent, string? name = null)
    {
        var inverter = new Inverter(name);
        parent.Attach(inverter);
        return inverter;
    }
}

