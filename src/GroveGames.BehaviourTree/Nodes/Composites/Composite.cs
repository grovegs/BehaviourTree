using System;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public class Composite : Node
{
    protected readonly IList<Node> childeren;

    public Composite(Blackboard blackboard) : base(blackboard)
    {
        childeren = [];
    }

    public Composite AddChild(Node child)
    {
        child.SetParent(this);
        childeren.Add(child);

        return this;
    }
}
