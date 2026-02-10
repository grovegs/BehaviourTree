using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree;

public interface IChildTree
{
    public void SetupTree(IParent parent);
}
