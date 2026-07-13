using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class SuccessOnceTests
{
    private sealed class TestBlackboard : IBlackboard
    {
        public bool TryGetValue<T>(BlackboardKey<T> key, out T value) { value = default!; return false; }
        public void SetValue<T>(BlackboardKey<T> key, T value) { }
        public void DeleteValue<T>(BlackboardKey<T> key) { }
        public void Clear() { }
    }

    private sealed class TestNode : INode
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
        public void Abort() { }
        public void StartEvaluate() { }
        public void EndEvaluate() { }
        public void SetParent(IParent parent) { }

        public void SetName(string name)
        {
        }
    }

    private sealed class TestParent : IParent
    {
        public IBlackboard Blackboard { get; } = new TestBlackboard();
        public IParent Attach(INode node) => this;
        public IParent Attach(IChildTree tree) => this;
    }

    [Fact]
    public void Evaluate_ShouldReturnSuccessOnFirstEvaluation()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var successOnce = new SuccessOnce();
        successOnce.Attach(child);

        var result = successOnce.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, successOnce.State);
    }

    [Fact]
    public void Evaluate_ShouldReturnFailureAfterFirstEvaluation()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var successOnce = new SuccessOnce();
        successOnce.Attach(child);

        var resultFirst = successOnce.Evaluate(1.0f);
        var resultSecond = successOnce.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, resultFirst);
        Assert.Equal(NodeState.Failure, resultSecond);
        Assert.Equal(NodeState.Failure, successOnce.State);
    }

    [Fact]
    public void Abort_ShouldAllowReEvaluation()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var successOnce = new SuccessOnce();
        successOnce.Attach(child);

        var resultFirst = successOnce.Evaluate(1.0f);
        successOnce.Abort();
        var resultAfterAbort = successOnce.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, resultFirst);
        Assert.Equal(NodeState.Success, resultAfterAbort);
        Assert.Equal(NodeState.Success, successOnce.State);
    }

    [Fact]
    public void Reset_ShouldAllowReEvaluation()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var successOnce = new SuccessOnce();
        successOnce.Attach(child);

        var resultFirst = successOnce.Evaluate(1.0f);
        successOnce.Reset();
        var resultAfterReset = successOnce.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, resultFirst);
        Assert.Equal(NodeState.Success, resultAfterReset);
        Assert.Equal(NodeState.Success, successOnce.State);
    }
}
