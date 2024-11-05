using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public sealed class Sequence : Composite
{
    private int _processingChildIndex;

    public Sequence(INode parent) : base(parent)
    {
        _processingChildIndex = 0;
    }

    public override NodeState Evaluate(IBlackboard blackboard, float deltaTime)
    {
        if (_processingChildIndex < Children.Count)
        {
            var child = Children[_processingChildIndex];

            child.BeforeEvaluate();
            var state = child.Evaluate(blackboard, deltaTime);

            switch (state)
            {
                case NodeState.Running:
                    return NodeState.Running;

                case NodeState.Failure:
                    _processingChildIndex = 0;
                    return NodeState.Failure;

                case NodeState.Success:
                    _processingChildIndex++;
                    return _processingChildIndex == Children.Count ? NodeState.Success : NodeState.Running;
            }

            child.AfterEvaluate();
        }

        Reset();
        return NodeState.Success;
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
