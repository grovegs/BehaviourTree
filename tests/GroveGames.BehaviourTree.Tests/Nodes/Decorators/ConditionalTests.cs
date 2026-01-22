using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class ConditionalTests
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
        public void SetParent(IParent parent) { }
    }

    private sealed class TestParent : IParent
    {
        public IBlackboard Blackboard { get; } = new TestBlackboard();
        public IParent Attach(INode node) => this;
        public IParent Attach(IChildTree tree) => this;
    }

    [Fact]
    public void Evaluate_ShouldReturnFailureWhenConditionIsFalse()
    {
        var parent = new TestParent();
        var child = new TestNode();
        var conditional = new Conditional(() => false);
        conditional.Attach(child);

        var result = conditional.Evaluate(1.0f);

        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, conditional.State);
        Assert.Equal(0, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldCallChildEvaluateWhenConditionIsTrue()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Running };
        var conditional = new Conditional(() => true);
        conditional.Attach(child);

        var result = conditional.Evaluate(1.0f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, conditional.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldNotCallChildEvaluateWhenConditionIsFalse()
    {
        var parent = new TestParent();
        var child = new TestNode();
        var conditional = new Conditional(() => false);
        conditional.Attach(child);

        conditional.Evaluate(1.0f);

        Assert.Equal(0, child.EvaluateCount);
    }
}
