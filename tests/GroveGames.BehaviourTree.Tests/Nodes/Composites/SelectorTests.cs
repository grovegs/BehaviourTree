using System.Reflection;

using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;

namespace GroveGames.BehaviourTree.Tests.Nodes.Composites;

public class SelectorTests
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

    private sealed class FakeParent : IParent
    {
        public IBlackboard Blackboard { get; } = new FakeBlackboard();
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
        var parent = new FakeParent();
        var selector = new Selector();
        var child1 = new FakeNode { ReturnState = NodeState.Failure };
        var child2 = new FakeNode { ReturnState = NodeState.Running };

        selector.Attach(child1).Attach(child2);

        var result = selector.Evaluate(0.1f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, selector.State);
        Assert.Equal(1, GetProcessingChildIndex(selector));
    }

    [Fact]
    public void Evaluate_ReturnsRunningIfCurrentChildIsRunning()
    {
        var parent = new FakeParent();
        var selector = new Selector();
        var child = new FakeNode { ReturnState = NodeState.Running };

        selector.Attach(child);

        var result = selector.Evaluate(0.1f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, selector.State);
        Assert.Equal(0, GetProcessingChildIndex(selector));
    }

    [Fact]
    public void Evaluate_ReturnsSuccessAndResetsIfAnyChildSucceeds()
    {
        var parent = new FakeParent();
        var selector = new Selector();
        var child1 = new FakeNode { ReturnState = NodeState.Failure };
        var child2 = new FakeNode { ReturnState = NodeState.Success };

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
        var parent = new FakeParent();
        var selector = new Selector();
        var child1 = new FakeNode { ReturnState = NodeState.Failure };
        var child2 = new FakeNode { ReturnState = NodeState.Failure };

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
        var parent = new FakeParent();
        var selector = new Selector();
        var child = new FakeNode { ReturnState = NodeState.Failure };

        selector.Attach(child);
        selector.Evaluate(0.1f);

        selector.Reset();

        Assert.Equal(0, GetProcessingChildIndex(selector));
    }

    [Fact]
    public void Abort_CallsAbortOnCurrentChildAndResetsProcessingChildIndex()
    {
        var parent = new FakeParent();
        var selector = new Selector();
        var child = new FakeNode { ReturnState = NodeState.Running };

        selector.Attach(child);
        selector.Evaluate(0.1f);

        selector.Abort();

        Assert.Equal(1, child.AbortCount);
        Assert.Equal(0, GetProcessingChildIndex(selector));
    }

    [Fact]
    public void Evaluate_ChildReturnsNone_TreatsItAsFailure()
    {
        var selector = new Selector();
        selector.Attach(new FakeNode { ReturnState = NodeState.None });

        var result = selector.Evaluate(0.1f);

        Assert.Equal(NodeState.Failure, result);
    }
}
