using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class CooldownTests
{
    private sealed class TestBlackboard : IBlackboard
    {
        public T? GetValue<T>(string key) => default;
        public void SetValue<T>(string key, T value) where T : notnull { }
        public void DeleteValue(string key) { }
        public void Clear() { }
    }

    private sealed class TestNode : INode
    {
        public int EvaluateCount { get; private set; }
        public NodeState ReturnState { get; set; } = NodeState.Success;
        public NodeState State => ReturnState;

        public NodeState Evaluate(float deltaTime)
        {
            EvaluateCount++;
            return ReturnState;
        }

        public void Reset() { }
        public void Abort() { }
        public void StartEvaluate() { }
        public void EndEvaluate() { }
    }

    private sealed class TestParent : IParent
    {
        public IBlackboard Blackboard { get; } = new TestBlackboard();
        public IParent Attach(INode node) => this;
        public IParent Attach(IChildTree tree) => this;
    }

    [Fact]
    public void Evaluate_ShouldReturnFailureWhenRemainingTimeIsGreaterThanZero()
    {
        var parent = new TestParent();
        var child = new TestNode();
        var cooldown = new Cooldown(parent, 2.0f);
        cooldown.Attach(child);

        cooldown.Evaluate(0.1f);
        var result = cooldown.Evaluate(1.0f);

        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, cooldown.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldReturnSuccessAfterCooldownExpires()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var cooldown = new Cooldown(parent, 2.0f);
        cooldown.Attach(child);

        cooldown.Evaluate(0.1f);
        var result = cooldown.Evaluate(2.0f);

        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, cooldown.State);
        Assert.Equal(2, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldCallChildEvaluateAfterCooldownExpires()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Running };
        var cooldown = new Cooldown(parent, 2.0f);
        cooldown.Attach(child);

        cooldown.Evaluate(0.1f);
        var result = cooldown.Evaluate(2.0f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, cooldown.State);
        Assert.Equal(2, child.EvaluateCount);
    }

    [Fact]
    public void Reset_ShouldResetRemainingTimeToZero()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var cooldown = new Cooldown(parent, 2.0f);
        cooldown.Attach(child);

        cooldown.Evaluate(1.0f);
        cooldown.Reset();
        var resultAfterReset = cooldown.Evaluate(0.5f);

        Assert.Equal(NodeState.Success, resultAfterReset);
        Assert.Equal(NodeState.Success, cooldown.State);
        Assert.Equal(2, child.EvaluateCount);
    }

    [Fact]
    public void Abort_ShouldResetRemainingTimeToZero()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var cooldown = new Cooldown(parent, 2.0f);
        cooldown.Attach(child);

        cooldown.Evaluate(1.0f);
        cooldown.Abort();
        var resultAfterAbort = cooldown.Evaluate(0.5f);

        Assert.Equal(NodeState.Success, resultAfterAbort);
        Assert.Equal(NodeState.Success, cooldown.State);
        Assert.Equal(2, child.EvaluateCount);
    }
}
