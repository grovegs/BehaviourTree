using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class RepeaterTests
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
    public void Evaluate_WithFixedCount_ShouldReturnRunningUntilMaxCountReached()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var repeater = new Repeater(RepeatMode.FixedCount, 2);
        repeater.Attach(child);

        var firstEvaluation = repeater.Evaluate(1.0f);
        var secondEvaluation = repeater.Evaluate(1.0f);
        var thirdEvaluation = repeater.Evaluate(1.0f);

        Assert.Equal(NodeState.Running, firstEvaluation);
        Assert.Equal(NodeState.Running, secondEvaluation);
        Assert.Equal(NodeState.Success, thirdEvaluation);
        Assert.Equal(NodeState.Success, repeater.State);
        Assert.Equal(2, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_WithUntilSuccess_ShouldReturnSuccessWhenChildSucceeds()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var repeater = new Repeater(RepeatMode.UntilSuccess);
        repeater.Attach(child);

        var result = repeater.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, repeater.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_WithUntilFailure_ShouldReturnFailureWhenChildFails()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Failure };
        var repeater = new Repeater(RepeatMode.UntilFailure);
        repeater.Attach(child);

        var result = repeater.Evaluate(1.0f);

        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_WithInfiniteRepeat_ShouldAlwaysReturnRunning()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Running };
        var repeater = new Repeater(RepeatMode.Infinite);
        repeater.Attach(child);

        var firstEvaluation = repeater.Evaluate(1.0f);
        var secondEvaluation = repeater.Evaluate(1.0f);

        Assert.Equal(NodeState.Running, firstEvaluation);
        Assert.Equal(NodeState.Running, secondEvaluation);
        Assert.Equal(NodeState.Running, repeater.State);
        Assert.Equal(2, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_WithReset_ShouldResetCurrentCount()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var repeater = new Repeater(RepeatMode.FixedCount, 2);
        repeater.Attach(child);

        repeater.Evaluate(1.0f);
        repeater.Evaluate(1.0f);

        repeater.Reset();
        var resultAfterReset = repeater.Evaluate(1.0f);

        Assert.Equal(NodeState.Running, resultAfterReset);
        Assert.Equal(NodeState.Running, repeater.State);
        Assert.Equal(3, child.EvaluateCount);
    }

    [Fact]
    public void Abort_ShouldResetCurrentCount()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var repeater = new Repeater(RepeatMode.FixedCount, 2);
        repeater.Attach(child);

        repeater.Evaluate(1.0f);
        repeater.Evaluate(1.0f);

        repeater.Abort();
        var resultAfterAbort = repeater.Evaluate(1.0f);

        Assert.Equal(NodeState.Running, resultAfterAbort);
        Assert.Equal(NodeState.Running, repeater.State);
        Assert.Equal(3, child.EvaluateCount);
    }
}
