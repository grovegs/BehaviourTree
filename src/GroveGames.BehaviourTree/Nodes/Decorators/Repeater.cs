using System;

using GroveGames.BehaviourTree;
using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators
{
    public enum RepeatMode
    {
        FixedCount,
        UntilSuccess,
        UntilFailure,
        Infinite
    }

    public sealed class Repeater : Decorator
    {
        private readonly RepeatMode _repeatMode;
        private readonly int _maxCount;
        private int _currentCount;

        public Repeater(Node child, RepeatMode repeatMode, int maxCount = -1) : base(child)
        {
            _repeatMode = repeatMode;
            _maxCount = maxCount;
            _currentCount = 0;
        }

        public override NodeState Evaluate(IBlackboard blackboard, double delta)
        {
            if (_repeatMode == RepeatMode.FixedCount && _currentCount >= _maxCount)
            {
                _currentCount = 0;
                return NodeState.SUCCESS;
            }

            var childStatus = base.Evaluate(blackboard, delta);

            switch (_repeatMode)
            {
                case RepeatMode.FixedCount:
                    if (childStatus == NodeState.SUCCESS || childStatus == NodeState.FAILURE)
                    {
                        _currentCount++;
                        return NodeState.RUNNING;
                    }
                    break;

                case RepeatMode.UntilSuccess:
                    if (childStatus == NodeState.SUCCESS)
                    {
                        return NodeState.SUCCESS;
                    }
                    return NodeState.RUNNING;

                case RepeatMode.UntilFailure:
                    if (childStatus == NodeState.FAILURE)
                    {
                        return NodeState.FAILURE;
                    }
                    return NodeState.RUNNING;

                case RepeatMode.Infinite:
                    return NodeState.RUNNING;
            }

            return childStatus == NodeState.RUNNING ? NodeState.RUNNING : NodeState.SUCCESS;
        }

        public override void Reset()
        {
            _currentCount = 0;
            child?.Reset();
        }

        public override void Abort()
        {
            _currentCount = 0;
            child?.Abort();
        }
    }
}
