using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class AbortTests
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
        public int AbortCount { get; private set; }
        public NodeState ReturnState { get; set; } = NodeState.Success;
        public NodeState State => ReturnState;

        public NodeState Evaluate(float deltaTime)
        {
            EvaluateCount++;
            return ReturnState;
        }

        public void Reset() { }
        public void Abort() => AbortCount++;
        public void StartEvaluate() { }
        public void EndEvaluate() { }
    }

    private sealed class TestParent : IParent
    {
        public IBlackboard Blackboard { get; } = new TestBlackboard();
        public IParent Attach(INode node) => this;
        public IParent Attach(IChildTree tree) => this;
    }

    [Fact]
    public void Evaluate_ShouldReturnSuccessWhenConditionIsTrue()
    {
        var parent = new TestParent();
        var child = new TestNode();
        var abortDecorator = new Abort(parent, () => true);
        abortDecorator.Attach(child);

        var result = abortDecorator.Evaluate(1.0f);

        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, abortDecorator.State);
        Assert.Equal(1, child.AbortCount);
    }

    [Fact]
    public void Evaluate_ShouldCallChildEvaluateWhenConditionIsFalse()
    {
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Running };
        var abortDecorator = new Abort(parent, () => false);
        abortDecorator.Attach(child);

        var result = abortDecorator.Evaluate(1.0f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, abortDecorator.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldNotCallChildEvaluateWhenConditionIsTrue()
    {
        var parent = new TestParent();
        var child = new TestNode();
        var abortDecorator = new Abort(parent, () => true);
        abortDecorator.Attach(child);

        abortDecorator.Evaluate(1.0f);

        Assert.Equal(0, child.EvaluateCount);
    }

    [Fact]
    public void Abort_ShouldCallChildAbortWhenConditionIsTrue()
    {
        var parent = new TestParent();
        var child = new TestNode();
        var abortDecorator = new Abort(parent, () => true);
        abortDecorator.Attach(child);

        abortDecorator.Evaluate(1.0f);

        Assert.Equal(1, child.AbortCount);
    }

    [Fact]
    public void Abort_ShouldNotCallChildAbortWhenConditionIsFalse()
    {
        var parent = new TestParent();
        var child = new TestNode();
        var abortDecorator = new Abort(parent, () => false);
        abortDecorator.Attach(child);

        abortDecorator.Evaluate(1.0f);

        Assert.Equal(0, child.AbortCount);
    }
}
