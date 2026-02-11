using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class DecoratorTests
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

    private sealed class TestParent : IParent
    {
        public IBlackboard Blackboard { get; } = new TestBlackboard();
        public IParent Attach(INode node) => this;
        public IParent Attach(IChildTree tree) => this;
    }

    private class TestDecorator : Decorator
    {
    }

    [Fact]
    public void Attach_ShouldSetChild_WhenChildIsEmpty()
    {
        var parent = new TestParent();
        var decorator = new TestDecorator();
        var node = new TestNode();

        decorator.Attach(node);

        Assert.Throws<ChildAlreadyAttachedException>(() => decorator.Attach(node));
    }

    [Fact]
    public void Attach_ShouldThrowException_WhenChildAlreadyAttached()
    {
        var parent = new TestParent();
        var decorator = new TestDecorator();
        var node1 = new TestNode();
        var node2 = new TestNode();

        decorator.Attach(node1);

        Assert.Throws<ChildAlreadyAttachedException>(() => decorator.Attach(node2));
    }

    [Fact]
    public void Evaluate_ShouldCallEvaluateOnChild()
    {
        var parent = new TestParent();
        var decorator = new TestDecorator();
        var node = new TestNode();
        decorator.Attach(node);

        decorator.Evaluate(0.5f);

        Assert.Equal(1, node.EvaluateCount);
        Assert.Equal(0.5f, node.LastDeltaTime);
    }

    [Fact]
    public void Abort_ShouldCallAbortOnChild()
    {
        var parent = new TestParent();
        var decorator = new TestDecorator();
        var node = new TestNode();
        decorator.Attach(node);

        decorator.Abort();

        Assert.Equal(1, node.AbortCount);
    }

    [Fact]
    public void Reset_ShouldCallResetOnChild()
    {
        var parent = new TestParent();
        var decorator = new TestDecorator();
        var node = new TestNode();
        decorator.Attach(node);

        decorator.Reset();

        Assert.Equal(1, node.ResetCount);
    }
}
