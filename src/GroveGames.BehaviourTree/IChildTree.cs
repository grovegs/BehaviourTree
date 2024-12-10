using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree;

public interface IChildTree
{
    INode SetupTree(IParent parent);
}
