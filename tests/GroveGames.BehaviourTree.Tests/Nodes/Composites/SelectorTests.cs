using System.Reflection;

using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;

namespace GroveGames.BehaviourTree.Tests.Nodes.Composites;

public class SelectorTests
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
        public int AbortCount { get; private set; }
        public NodeState ReturnState { get; set; } = NodeState.Success;
        public NodeState State => ReturnState;

        public string Name => string.Empty;

        public NodeState Evaluate(float deltaTime)
        {
            EvaluateCount++;
            return ReturnState;
        }

        public void Reset() { }
        public void Abort() => AbortCount++;
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

    private static int GetProcessingChildIndex(Selector selector)
    {
        var fieldInfo = typeof(Selector).GetField("_processingChildIndex", BindingFlags.NonPublic | BindingFlags.Instance);
        return (int)fieldInfo?.GetValue(selector)!;
    }

    [Fact]
    public void Evaluate_ReturnsRunningAndMovesToNextChildOnFailure()
    {
        var parent = new TestParent();
        var selector = new Selector();
        var child1 = new TestNode { ReturnState = NodeState.Failure };
        var child2 = new TestNode { ReturnState = NodeState.Running };

        selector.Attach(child1).Attach(child2);

        var result = selector.Evaluate(0.1f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, selector.State);
        Assert.Equal(1, GetProcessingChildIndex(selector));
    }

    [Fact]
    public void Evaluate_ReturnsRunningIfCurrentChildIsRunning()
    {
        var parent = new TestParent();
        var selector = new Selector();
        var child = new TestNode { ReturnState = NodeState.Running };

        selector.Attach(child);

        var result = selector.Evaluate(0.1f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, selector.State);
        Assert.Equal(0, GetProcessingChildIndex(selector));
    }

    [Fact]
    public void Evaluate_ReturnsSuccessAndResetsIfAnyChildSucceeds()
    {
        var parent = new TestParent();
        var selector = new Selector();
        var child1 = new TestNode { ReturnState = NodeState.Failure };
        var child2 = new TestNode { ReturnState = NodeState.Success };

        selector.Attach(child1).Attach(child2);

        selector.Evaluate(0.1f);
        var result = selector.Evaluate(0.1f);

        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, selector.State);
        Assert.Equal(0, GetProcessingChildIndex(selector));
    }

    [Fact]
    public void Evaluate_ReturnsFailureIfAllChildrenFail()
    {
        var parent = new TestParent();
        var selector = new Selector();
        var child1 = new TestNode { ReturnState = NodeState.Failure };
        var child2 = new TestNode { ReturnState = NodeState.Failure };

        selector.Attach(child1).Attach(child2);

        selector.Evaluate(0.1f);
        selector.Evaluate(0.1f);
        var result = selector.Evaluate(0.1f);

        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, selector.State);
        Assert.Equal(0, GetProcessingChildIndex(selector));
    }

    [Fact]
    public void Reset_ResetsProcessingChildIndex()
    {
        var parent = new TestParent();
        var selector = new Selector();
        var child = new TestNode { ReturnState = NodeState.Failure };

        selector.Attach(child);
        selector.Evaluate(0.1f);

        selector.Reset();

        Assert.Equal(0, GetProcessingChildIndex(selector));
    }

    [Fact]
    public void Abort_CallsAbortOnCurrentChildAndResetsProcessingChildIndex()
    {
        var parent = new TestParent();
        var selector = new Selector();
        var child = new TestNode { ReturnState = NodeState.Running };

        selector.Attach(child);
        selector.Evaluate(0.1f);

        selector.Abort();

        Assert.Equal(1, child.AbortCount);
        Assert.Equal(0, GetProcessingChildIndex(selector));
    }
}
