using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes;

public interface IParent
{
    IParent Attach(INode node);
    IParent Attach(IChildTree tree);
    IBlackboard Blackboard { get; }
}
