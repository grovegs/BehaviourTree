using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;

namespace GroveGames.BehaviourTree.Tests.Nodes.Composites;

public class SequenceTests
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
    public void Evaluate_ShouldReturnRunningWhenFirstChildIsRunning()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Running };
        var sequence = new Sequence();
        sequence.Attach(child);

        var result = sequence.Evaluate(1.0f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, sequence.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldReturnFailureWhenChildFails()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Failure };
        var sequence = new Sequence();
        sequence.Attach(child);

        var result = sequence.Evaluate(1.0f);

        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, sequence.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldReturnSuccessWhenAllChildrenSucceed()
    {
        var parent = new TestParent();
        var child1 = new TestNode { ReturnState = NodeState.Success };
        var child2 = new TestNode { ReturnState = NodeState.Success };
        var sequence = new Sequence();
        sequence.Attach(child1).Attach(child2);

        sequence.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, sequence.State);
        Assert.Equal(1, child1.EvaluateCount);
        Assert.Equal(1, child2.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldResetOnFailureAfterSuccess()
    {
        var parent = new TestParent();
        var child1 = new TestNode { ReturnState = NodeState.Success };
        var child2 = new TestNode { ReturnState = NodeState.Failure };
        var sequence = new Sequence();
        sequence.Attach(child1).Attach(child2);

        sequence.Evaluate(1.0f);

        Assert.Equal(NodeState.Failure, sequence.State);
        Assert.Equal(1, child1.EvaluateCount);
        Assert.Equal(1, child2.EvaluateCount);
    }

    [Fact]
    public void Reset_ShouldResetProcessingChildIndex()
    {
        var parent = new TestParent();
        var child1 = new TestNode { ReturnState = NodeState.Success };
        var child2 = new TestNode { ReturnState = NodeState.Success };
        var sequence = new Sequence();
        sequence.Attach(child1).Attach(child2);

        sequence.Evaluate(1.0f);
        sequence.Reset();
        var resetResult = sequence.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, resetResult);
        Assert.Equal(2, child1.EvaluateCount);
    }

    [Fact]
    public void Abort_ShouldResetProcessingChildIndex()
    {
        var parent = new TestParent();
        var child1 = new TestNode { ReturnState = NodeState.Success };
        var child2 = new TestNode { ReturnState = NodeState.Success };
        var sequence = new Sequence();
        sequence.Attach(child1).Attach(child2);

        sequence.Evaluate(1.0f);
        sequence.Abort();
        var resultAfterAbort = sequence.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, resultAfterAbort);
        Assert.Equal(2, child1.EvaluateCount);
    }
}
