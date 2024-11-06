using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Nodes.Decorators
{

    public sealed class Repeater : Decorator
    {
        private readonly RepeatMode _repeatMode;
        private readonly int _maxCount;
        private int _currentCount;

        public Repeater(IParent parent, IBlackboard blackboard, RepeatMode repeatMode, int maxCount = -1) : base(parent, blackboard)
        {
            _repeatMode = repeatMode;
            _maxCount = maxCount;
            _currentCount = 0;
        }

        public override NodeState Evaluate(float deltaTime)
        {
            if (_repeatMode == RepeatMode.FixedCount && _currentCount >= _maxCount)
            {
                _currentCount = 0;
                return NodeState.Success;
            }

            var childStatus = base.Evaluate(deltaTime);

            switch (_repeatMode)
            {
                case RepeatMode.FixedCount:
                    if (childStatus == NodeState.Success || childStatus == NodeState.Failure)
                    {
                        _currentCount++;
                        return NodeState.Running;
                    }
                    break;

                case RepeatMode.UntilSuccess:
                    if (childStatus == NodeState.Success)
                    {
                        return NodeState.Success;
                    }
                    return NodeState.Running;

                case RepeatMode.UntilFailure:
                    if (childStatus == NodeState.Failure)
                    {
                        return NodeState.Failure;
                    }
                    return NodeState.Running;

                case RepeatMode.Infinite:
                    return NodeState.Running;
            }

            return childStatus == NodeState.Running ? NodeState.Running : NodeState.Success;
        }

        public override void Reset()
        {
            base.Reset();
            _currentCount = 0;
        }

        public override void Abort()
        {
            base.Abort();
            _currentCount = 0;
        }
    }
}

public static partial class ParentExtensions
{
    public static void AttachRepeater(this IParent parent, RepeatMode repeatMode)
    {
        var repeater = new Repeater(parent, parent.Blackboard, repeatMode);
        parent.Attach(repeater);
    }
}
