using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public sealed class Selector : Composite
{
    private int _processingChildIndex;

    public Selector(INode parent) : base(parent)
    {
        _processingChildIndex = 0;
    }

    public override NodeState Evaluate(IBlackboard blackboard, float deltaTime)
    {
        if (_processingChildIndex < Children.Count)
        {
            var child = Children[_processingChildIndex];

            child.BeforeEvaluate();

            var status = child.Evaluate(blackboard, deltaTime);

            switch (status)
            {
                case NodeState.Failure:
                    _processingChildIndex++;
                    return NodeState.Running;

                case NodeState.Running:
                    return NodeState.Running;

                case NodeState.Success:
                    Reset();
                    return NodeState.Success;
            }

            child.AfterEvaluate();
        }

        Reset();
        return NodeState.Failure;
    }

    public override void Reset()
    {
        base.Reset();
        _processingChildIndex = 0;
    }

    public override void Abort()
    {
        if (_processingChildIndex < Children.Count)
        {
            Children[_processingChildIndex].Abort();
        }
    }
}
