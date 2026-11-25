using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class ConditionalTests
{
    private sealed class FakeBlackboard : IBlackboard
    {
        public bool TryGetValue<T>(BlackboardKey<T> key, out T value) { value = default!; return false; }
        public void SetValue<T>(BlackboardKey<T> key, T value) { }
        public void DeleteValue<T>(BlackboardKey<T> key) { }
        public void Clear() { }
    }

    private sealed class FakeNode : INode
    {
        public int EvaluateCount { get; private set; }
        public NodeState ReturnState { get; set; } = NodeState.Success;
        public NodeState State => ReturnState;

        public string Name => string.Empty;

        public NodeState Evaluate(float deltaTime)
        {
            EvaluateCount++;
            return ReturnState;
        }

        public void Reset() { }
        public int AbortCount { get; private set; }
        public void Abort() => AbortCount++;
        public void StartEvaluate() { }
        public void EndEvaluate() { }
        public void SetParent(IParent parent) { }

        public void SetName(string name)
        {
        }
    }

    private sealed class FakeParent : IParent
    {
        public IBlackboard Blackboard { get; } = new FakeBlackboard();
        public IParent Attach(INode node) => this;
        public IParent Attach(IChildTree tree) => this;
    }

    [Fact]
    public void Evaluate_ShouldReturnFailureWhenConditionIsFalse()
    {
        var parent = new FakeParent();
        var child = new FakeNode();
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
        var parent = new FakeParent();
        var child = new FakeNode { ReturnState = NodeState.Running };
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
        var parent = new FakeParent();
        var child = new FakeNode();
        var conditional = new Conditional(() => false);
        conditional.Attach(child);

        conditional.Evaluate(1.0f);

        Assert.Equal(0, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ConditionTurnsFalseWhileChildRuns_AbortsTheChild()
    {
        var isAllowed = true;
        var child = new FakeNode { ReturnState = NodeState.Running };
        var conditional = new Conditional(() => isAllowed);
        conditional.Attach(child);

        conditional.Evaluate(0.1f);
        isAllowed = false;
        var result = conditional.Evaluate(0.1f);

        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(1, child.AbortCount);
    }
}
