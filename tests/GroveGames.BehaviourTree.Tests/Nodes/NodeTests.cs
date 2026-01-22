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
        public T GetValue<T>(string key) => default!;
        public void SetValue<T>(string key, T value) where T : notnull { }
        public bool ContainsKey(string key) => false;
        public void DeleteValue(string key) { }
        public void Clear() { }
    }

    private sealed class TestNode : BehaviourNode
    {
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
