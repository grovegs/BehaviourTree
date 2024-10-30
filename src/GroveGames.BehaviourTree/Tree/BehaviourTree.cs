using System;

using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree.Tree;

public abstract class BehaviourTree
{
    protected Node root;

    public abstract void SetupTree();

    public virtual void Tick()
    {
        if (root != null)
        {
            root.Evaluate();
        }
    }
}
