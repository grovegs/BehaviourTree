using System;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public class Composite : Node
{
    protected readonly IList<Node> childeren;
    protected int processingChild;

    public Composite() : base()
    {
        childeren = [];
    }

    public Composite AddChild(Node child)
    {
        child.SetParent(this);
        childeren.Add(child);

        return this;
    }

    public override void Interrupt()
    {
        childeren[processingChild % childeren.Count].Interrupt();
        processingChild = 0;
    }
}
