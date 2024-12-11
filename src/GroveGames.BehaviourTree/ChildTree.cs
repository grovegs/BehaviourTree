using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree;

public abstract class ChildTree : IChildTree
{
    public abstract void SetupTree(IParent parent);
}
