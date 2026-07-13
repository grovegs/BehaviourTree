using System.Reflection;

using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree.Tests.Nodes;

public class NodeTests
{
    private sealed class NodeAccessor
    {
        public static IParent Parent(BehaviourNode node)
        {
            var fieldInfo = typeof(BehaviourNode).GetProperty("Parent", BindingFlags.NonPublic | BindingFlags.Instance);
            return (IParent)fieldInfo?.GetValue(node)!;
        }
    }

    private sealed class TestParent : IParent
    {
        public IBlackboard Blackboard { get; set; } = new TestBlackboard();

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

    private sealed class TestNode : BehaviourNode
    {
        public TestNode(string? name = null) : base(name) { }
    }

    [Fact]
    public void Name_ShouldReturnTypeName_WhenNoNameProvided()
    {
        var node = new TestNode();

        Assert.Equal(nameof(TestNode), node.Name);
    }

    [Fact]
    public void Name_ShouldReturnCustomName_WhenNameProvidedInConstructor()
    {
        var node = new TestNode("MyNode");

        Assert.Equal("MyNode", node.Name);
    }

    [Fact]
    public void SetName_ShouldOverrideName()
    {
        var node = new TestNode();
        node.SetName("UpdatedName");

        Assert.Equal("UpdatedName", node.Name);
    }

    [Fact]
    public void Evaluate_ShouldReturnFailure_ByDefault()
    {
        var node = new TestNode();

        var result = node.Evaluate(1.0f);

        Assert.Equal(NodeState.Failure, result);
    }

    [Fact]
    public void Reset_ShouldNotThrowException()
    {
        var node = new TestNode();

        var exception = Record.Exception(node.Reset);
        Assert.Null(exception);
    }

    [Fact]
    public void Abort_ShouldNotThrowException()
    {
        var node = new TestNode();

        var exception = Record.Exception(node.Abort);
        Assert.Null(exception);
    }

    [Fact]
    public void Parent_ShouldReturnParent()
    {
        var parent = new TestParent();
        var node = new TestNode();
        node.SetParent(parent);

        var result = NodeAccessor.Parent(node);

        Assert.Equal(parent, result);
    }

    [Fact]
    public void Blackboard_ShouldReturnBlackboardFromParent()
    {
        var blackboard = new TestBlackboard();
        var parent = new TestParent { Blackboard = blackboard };
        var node = new TestNode();
        node.SetParent(parent);

        var result = node.Blackboard;

        Assert.Equal(blackboard, result);
    }
}
