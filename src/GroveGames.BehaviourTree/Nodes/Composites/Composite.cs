using System;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public class Composite : Node
{
    protected readonly IList<Node> children;
    protected int processingChildIndex;

    public Composite() : base()
    {
        children = [];
    }

    public Composite AddChild(Node child)
    {
        child.SetParent(this);
        children.Add(child);

        return this;
    }

    public override void Reset()
    {
        processingChildIndex = 0;

        foreach (var child in children)
        {
            child.Reset();
        }
    }

    public override void Abort()
    {
        if (processingChildIndex < children.Count)
        {
            children[processingChildIndex].Abort();
        }

        Reset();
    }
}
