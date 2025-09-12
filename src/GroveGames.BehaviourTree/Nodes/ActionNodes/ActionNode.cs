using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroveGames.BehaviourTree.Nodes.ActionNode;
public class ActionNode : BehaviourNode
{
    private readonly Func<NodeState> _action;
    public ActionNode(IParent parent, Func<NodeState> action) : base(parent)
    {
        _action = action;
    }
    public override NodeState Evaluate(float deltaTime)
    {
        return _action.Invoke();
    }
}
