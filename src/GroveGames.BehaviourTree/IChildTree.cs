using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree;

public interface IChildTree : INode
{
    void SetupTree();
}
