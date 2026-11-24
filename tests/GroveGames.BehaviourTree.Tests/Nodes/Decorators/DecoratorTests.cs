using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class DecoratorTests
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
        public int ResetCount { get; private set; }
        public int AbortCount { get; private set; }
        public float LastDeltaTime { get; private set; }
        public NodeState State => NodeState.Success;

        public string Name => string.Empty;

        public NodeState Evaluate(float deltaTime)
        {
            EvaluateCount++;
            LastDeltaTime = deltaTime;
            return NodeState.Success;
        }

        public void Reset() => ResetCount++;
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

    private class TestDecorator : Decorator
    {
    }

    [Fact]
    public void Attach_ShouldSetChild_WhenChildIsEmpty()
    {
        var parent = new FakeParent();
        var decorator = new TestDecorator();
        var node = new FakeNode();

        decorator.Attach(node);

        Assert.Throws<ChildAlreadyAttachedException>(() => decorator.Attach(node));
    }

    [Fact]
    public void Attach_ShouldThrowException_WhenChildAlreadyAttached()
    {
        var parent = new FakeParent();
        var decorator = new TestDecorator();
        var node1 = new FakeNode();
        var node2 = new FakeNode();

        decorator.Attach(node1);

        Assert.Throws<ChildAlreadyAttachedException>(() => decorator.Attach(node2));
    }

    [Fact]
    public void Evaluate_ShouldCallEvaluateOnChild()
    {
        var parent = new FakeParent();
        var decorator = new TestDecorator();
        var node = new FakeNode();
        decorator.Attach(node);

        decorator.Evaluate(0.5f);

        Assert.Equal(1, node.EvaluateCount);
        Assert.Equal(0.5f, node.LastDeltaTime);
    }

    [Fact]
    public void Abort_ShouldCallAbortOnChild()
    {
        var parent = new FakeParent();
        var decorator = new TestDecorator();
        var node = new FakeNode();
        decorator.Attach(node);

        decorator.Abort();

        Assert.Equal(1, node.AbortCount);
    }

    [Fact]
    public void Reset_ShouldCallResetOnChild()
    {
        var parent = new FakeParent();
        var decorator = new TestDecorator();
        var node = new FakeNode();
        decorator.Attach(node);

        decorator.Reset();

        Assert.Equal(1, node.ResetCount);
    }
}
