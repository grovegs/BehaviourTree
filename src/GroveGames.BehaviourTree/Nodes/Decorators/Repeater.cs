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
            while (true)
            {
                var childStatus = base.Evaluate(blackboard, delta);

                switch (_repeatMode)
                {
                    case RepeatMode.FixedCount:
                        if (_currentCount >= _maxCount)
                        {
                            _currentCount = 0;
                            return NodeState.SUCCESS;
                        }
                        _currentCount++;
                        break;

                    case RepeatMode.UntilSuccess:
                        if (childStatus == NodeState.SUCCESS)
                        {
                            return NodeState.SUCCESS;
                        }
                        break;

                    case RepeatMode.UntilFailure:
                        if (childStatus == NodeState.FAILURE)
                        {
                            return NodeState.FAILURE;
                        }
                        break;

                    case RepeatMode.Infinite:
                        return NodeState.RUNNING;
                }

                if (childStatus == NodeState.RUNNING)
                {
                    return NodeState.RUNNING;
                }
            }
        }

        public override void Interrupt()
        {
            _currentCount = 0;
            child.Interrupt();
        }
    }
}
