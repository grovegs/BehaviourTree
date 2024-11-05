using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Cooldown : Decorator
{
    private readonly float _waitTime;
    private float _remainingTime;

    public Cooldown(INode parent, INode child, float waitTime) : base(parent, child)
    {
        _waitTime = waitTime;
        _remainingTime = 0;
    }

    public override NodeState Evaluate(IBlackboard blackboard, float deltaTime)
    {
        if (_remainingTime > 0f)
        {
            _remainingTime -= deltaTime;
            return NodeState.Failure;
        }

        _remainingTime = _waitTime;
        return base.Evaluate(blackboard, deltaTime);
    }

    public override void Reset()
    {
        base.Reset();
        _remainingTime = 0f;
    }

    public override void Abort()
    {
        base.Abort();
        _remainingTime = 0f;
    }
}
