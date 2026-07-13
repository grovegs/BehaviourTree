using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class InverterTests
{
    private sealed class TestBlackboard : IBlackboard
    {
        public bool TryGetValue<T>(BlackboardKey<T> key, out T value) { value = default!; return false; }
        public void SetValue<T>(BlackboardKey<T> key, T value) { }
        public void DeleteValue<T>(BlackboardKey<T> key) { }
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
    public void Evaluate_ShouldReturnSuccessWhenChildReturnsFailure()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Failure };
        var inverter = new Inverter();
        inverter.Attach(child);

        var result = inverter.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, inverter.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldReturnFailureWhenChildReturnsSuccess()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var inverter = new Inverter();
        inverter.Attach(child);

        var result = inverter.Evaluate(1.0f);

        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, inverter.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldReturnRunningWhenChildReturnsRunning()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Running };
        var inverter = new Inverter();
        inverter.Attach(child);

        var result = inverter.Evaluate(1.0f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, inverter.State);
        Assert.Equal(1, child.EvaluateCount);
    }
}
