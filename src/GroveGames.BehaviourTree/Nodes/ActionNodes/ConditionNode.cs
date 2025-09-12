using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroveGames.BehaviourTree.Nodes.ActionNodes;
public class ConditionNode : BehaviourNode
{
    private readonly Func<bool> _condition;
    public ConditionNode(IParent parent, Func<bool> condition) : base(parent)
    {
        _condition = condition;
    }
    public override NodeState Evaluate(float deltaTime)
    {
        if (_condition())
            return NodeState.Success;
        else return NodeState.Failure;
    }
}
