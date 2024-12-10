using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public interface IParent
{
    IParent Attach(INode node);
    IParent Attach(ITree tree);
    IBlackboard Blackboard { get; }
}
