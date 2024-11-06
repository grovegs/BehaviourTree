using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Composites;

public sealed class Sequence : Composite
{
    private int _processingChildIndex;

    public Sequence(IParent parent, IBlackboard blackboard) : base(parent, blackboard)
    {
        _processingChildIndex = 0;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (_processingChildIndex < Children.Count)
        {
            var child = Children[_processingChildIndex];
            var state = child.Evaluate(deltaTime);

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

        _processingChildIndex = 0;
    }
}

public static partial class ParentExtensions
{
    public static IParent AttachSequence(this IParent parent)
    {
        var sequence = new Sequence(parent, parent.Blackboard);
        parent.Attach(sequence);
        return parent;
    }
}
