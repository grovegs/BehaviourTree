using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Nodes.Decorators;

public sealed class Cooldown : Decorator
{
    private readonly float _waitTime;
    private float _remainingTime;

    public Cooldown(INode parent, IBlackboard blackboard, INode child, float waitTime) : base(parent, blackboard, child)
    {
        _waitTime = waitTime;
        _remainingTime = 0;
    }

    public override NodeState Evaluate(float deltaTime)
    {
        if (_remainingTime > 0f)
        {
            _remainingTime -= deltaTime;
            return NodeState.Failure;
        }

        _remainingTime = _waitTime;
        return base.Evaluate(deltaTime);
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
