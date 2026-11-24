using System.Reflection;

using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;

namespace GroveGames.BehaviourTree.Tests.Nodes.Composites;

public class CompositeTests
{
    private sealed class CompositeAccessor
    {
        public static IReadOnlyList<INode> Children(Composite composite)
        {
            var fieldInfo = typeof(Selector).GetProperty("Children", BindingFlags.NonPublic | BindingFlags.Instance);
            return (IReadOnlyList<INode>)fieldInfo?.GetValue(composite)!;
        }
    }

    private sealed class TestComposite : Composite
    {
    }

    private sealed class FakeParent : IParent
    {
        public IBlackboard Blackboard { get; } = new FakeBlackboard();

        public IParent Attach(INode node)
        {
            return this;
        }

        public IParent Attach(IChildTree tree)
        {
            return this;
        }
    }

    private sealed class FakeBlackboard : IBlackboard
    {
        public bool TryGetValue<T>(BlackboardKey<T> key, out T value) { value = default!; return false; }
        public void SetValue<T>(BlackboardKey<T> key, T value) { }
        public void DeleteValue<T>(BlackboardKey<T> key) { }
        public void Clear() { }
    }

    private sealed class FakeNode : INode
    {
        public int ResetCallCount { get; private set; }
        public NodeState State { get; set; } = NodeState.Success;

        public string Name => string.Empty;

        public NodeState Evaluate(float deltaTime) => State;

        public void Reset()
        {
            ResetCallCount++;
        }

        public void Abort() { }
        public void StartEvaluate() { }
        public void EndEvaluate() { }
        public void SetParent(IParent parent) { }

        public void SetName(string name)
        {
        }
    }

    [Fact]
    public void Attach_AddsNodeToChildren()
    {
        var parent = new FakeParent();
        var node = new FakeNode();
        var composite = new TestComposite();

        composite.Attach(node);

        Assert.Contains(node, CompositeAccessor.Children(composite));
    }

    [Fact]
    public void Reset_CallsResetOnAllChildren()
    {
        var parent = new FakeParent();
        var child1 = new FakeNode();
        var child2 = new FakeNode();
        var composite = new TestComposite();

        composite.Attach(child1);
        composite.Attach(child2);

        composite.Reset();

        Assert.Equal(1, child1.ResetCallCount);
        Assert.Equal(1, child2.ResetCallCount);
    }
}
