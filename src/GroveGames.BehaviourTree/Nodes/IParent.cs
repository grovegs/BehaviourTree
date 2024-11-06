using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public interface IParent
{
    IParent Attach(INode node);
    IBlackboard Blackboard { get; }
}
