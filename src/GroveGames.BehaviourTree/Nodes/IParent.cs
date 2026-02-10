using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public interface IParent
{
    public IParent Attach(INode node);
    public IParent Attach(IChildTree tree);
    public IBlackboard Blackboard { get; }
}
