using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class SucceederTests
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

        public string Name => string.Empty;

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

    [Fact]
    public void Evaluate_WithChildRunning_ShouldReturnRunning()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Running };
        var succeeder = new Succeeder();
        succeeder.Attach(child);

        var result = succeeder.Evaluate(1.0f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, succeeder.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_WithChildSuccess_ShouldReturnSuccess()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var succeeder = new Succeeder();
        succeeder.Attach(child);

        var result = succeeder.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, succeeder.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_WithChildFailure_ShouldReturnSuccess()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Failure };
        var succeeder = new Succeeder();
        succeeder.Attach(child);

        var result = succeeder.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, succeeder.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldCallChildEvaluateOnce()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Failure };
        var succeeder = new Succeeder();
        succeeder.Attach(child);

        succeeder.Evaluate(1.0f);

        Assert.Equal(1, child.EvaluateCount);
    }
}
