using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class AbortTests
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

    [Fact]
    public void Evaluate_ShouldReturnSuccessWhenConditionIsTrue()
    {
        var parent = new FakeParent();
        var child = new FakeNode();
        var abortDecorator = new Abort(() => true);
        abortDecorator.Attach(child);

        var result = abortDecorator.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, abortDecorator.State);
        Assert.Equal(1, child.AbortCount);
    }

    [Fact]
    public void Evaluate_ShouldCallChildEvaluateWhenConditionIsFalse()
    {
        var parent = new FakeParent();
        var child = new FakeNode { ReturnState = NodeState.Running };
        var abortDecorator = new Abort(() => false);
        abortDecorator.Attach(child);

        var result = abortDecorator.Evaluate(1.0f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, abortDecorator.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldNotCallChildEvaluateWhenConditionIsTrue()
    {
        var parent = new FakeParent();
        var child = new FakeNode();
        var abortDecorator = new Abort(() => true);
        abortDecorator.Attach(child);

        abortDecorator.Evaluate(1.0f);

        Assert.Equal(0, child.EvaluateCount);
    }

    [Fact]
    public void Abort_ShouldCallChildAbortWhenConditionIsTrue()
    {
        var parent = new FakeParent();
        var child = new FakeNode();
        var abortDecorator = new Abort(() => true);
        abortDecorator.Attach(child);

        abortDecorator.Evaluate(1.0f);

        Assert.Equal(1, child.AbortCount);
    }

    [Fact]
    public void Abort_ShouldNotCallChildAbortWhenConditionIsFalse()
    {
        var parent = new FakeParent();
        var child = new FakeNode();
        var abortDecorator = new Abort(() => false);
        abortDecorator.Attach(child);

        abortDecorator.Evaluate(1.0f);

        Assert.Equal(0, child.AbortCount);
    }
}
