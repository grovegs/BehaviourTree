using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class ResetTests
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
        public NodeState ReturnState { get; set; } = NodeState.Success;
        public NodeState State => ReturnState;

        public NodeState Evaluate(float deltaTime)
        {
            EvaluateCount++;
            return ReturnState;
        }

        public void Reset() => ResetCount++;
        public void Abort() { }
        public void StartEvaluate() { }
        public void EndEvaluate() { }
        public void SetParent(IParent parent) { }
    }

    private sealed class TestParent : IParent
    {
        public IBlackboard Blackboard { get; } = new TestBlackboard();
        public IParent Attach(INode node) => this;
        public IParent Attach(IChildTree tree) => this;
    }

    [Fact]
    public void Evaluate_WithConditionTrue_ShouldCallResetAndReturnSuccess()
    {
        var parent = new TestParent();
        var child = new TestNode();
        var resetDecorator = new Reset(() => true);
        resetDecorator.Attach(child);

        var result = resetDecorator.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, resetDecorator.State);
        Assert.Equal(0, child.EvaluateCount);
        Assert.Equal(1, child.ResetCount);
    }

    [Fact]
    public void Evaluate_WithConditionFalse_ShouldNotCallResetAndReturnChildState()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Running };
        var resetDecorator = new Reset(() => false);
        resetDecorator.Attach(child);

        var result = resetDecorator.Evaluate(1.0f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, resetDecorator.State);
        Assert.Equal(1, child.EvaluateCount);
        Assert.Equal(0, child.ResetCount);
    }

    [Fact]
    public void Evaluate_WithConditionTrue_ShouldResetChildWhenCalledMultipleTimes()
    {
        var parent = new TestParent();
        var child = new TestNode();
        var resetDecorator = new Reset(() => true);
        resetDecorator.Attach(child);

        resetDecorator.Evaluate(1.0f);
        resetDecorator.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, resetDecorator.State);
        Assert.Equal(0, child.EvaluateCount);
        Assert.Equal(2, child.ResetCount);
    }

    [Fact]
    public void Reset_ShouldNotCallChildResetWhenConditionIsFalse()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Running };
        var resetDecorator = new Reset(() => false);
        resetDecorator.Attach(child);

        resetDecorator.Evaluate(1.0f);

        Assert.Equal(1, child.EvaluateCount);
        Assert.Equal(0, child.ResetCount);
    }
}
