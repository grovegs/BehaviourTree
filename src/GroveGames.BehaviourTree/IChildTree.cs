using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree;

public interface IChildTree
{
    void SetupTree(IParent parent);
}
