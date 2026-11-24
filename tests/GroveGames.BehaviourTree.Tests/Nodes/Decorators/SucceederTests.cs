using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class SucceederTests
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

    private sealed class FakeParent : IParent
    {
        public IBlackboard Blackboard { get; } = new FakeBlackboard();
        public IParent Attach(INode node) => this;
        public IParent Attach(IChildTree tree) => this;
    }

    [Fact]
    public void Evaluate_WithChildRunning_ShouldReturnRunning()
    {
        var parent = new FakeParent();
        var child = new FakeNode { ReturnState = NodeState.Running };
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
        var parent = new FakeParent();
        var child = new FakeNode { ReturnState = NodeState.Success };
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
        var parent = new FakeParent();
        var child = new FakeNode { ReturnState = NodeState.Failure };
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
        var parent = new FakeParent();
        var child = new FakeNode { ReturnState = NodeState.Failure };
        var succeeder = new Succeeder();
        succeeder.Attach(child);

        succeeder.Evaluate(1.0f);

        Assert.Equal(1, child.EvaluateCount);
    }
}
