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

    private sealed class TestParent : IParent
    {
        public IBlackboard Blackboard { get; } = new TestBlackboard();

        public IParent Attach(INode node)
        {
            return this;
        }

        public IParent Attach(IChildTree tree)
        {
            return this;
        }
    }

    private sealed class TestBlackboard : IBlackboard
    {
        public bool TryGetValue<T>(BlackboardKey<T> key, out T value) { value = default!; return false; }
        public void SetValue<T>(BlackboardKey<T> key, T value) { }
        public void DeleteValue<T>(BlackboardKey<T> key) { }
        public void Clear() { }
    }

    private sealed class TestNode : INode
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
        var parent = new TestParent();
        var node = new TestNode();
        var composite = new TestComposite();

        composite.Attach(node);

        Assert.Contains(node, CompositeAccessor.Children(composite));
    }

    [Fact]
    public void Reset_CallsResetOnAllChildren()
    {
        var parent = new TestParent();
        var child1 = new TestNode();
        var child2 = new TestNode();
        var composite = new TestComposite();

        composite.Attach(child1);
        composite.Attach(child2);

        composite.Reset();

        Assert.Equal(1, child1.ResetCallCount);
        Assert.Equal(1, child2.ResetCallCount);
    }
}
