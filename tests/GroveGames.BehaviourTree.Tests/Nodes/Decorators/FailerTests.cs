using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class FailerTests
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
    public void Evaluate_ShouldReturnFailureWhenChildReturnsSuccess()
    {
        var parent = new FakeParent();
        var child = new FakeNode { ReturnState = NodeState.Success };
        var failer = new Failer();
        failer.Attach(child);

        var result = failer.Evaluate(1.0f);

        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, failer.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldReturnFailureWhenChildReturnsFailure()
    {
        var parent = new FakeParent();
        var child = new FakeNode { ReturnState = NodeState.Failure };
        var failer = new Failer();
        failer.Attach(child);

        var result = failer.Evaluate(1.0f);

        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, failer.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldReturnRunningWhenChildReturnsRunning()
    {
        var parent = new FakeParent();
        var child = new FakeNode { ReturnState = NodeState.Running };
        var failer = new Failer();
        failer.Attach(child);

        var result = failer.Evaluate(1.0f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, failer.State);
        Assert.Equal(1, child.EvaluateCount);
    }
}
