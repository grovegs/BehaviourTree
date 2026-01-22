using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class DelayerTests
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

        public NodeState Evaluate(float deltaTime)
        {
            EvaluateCount++;
            return ReturnState;
        }

        public void Reset() { }
        public void Abort() { }
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
    public void Evaluate_ShouldReturnRunningWhenIntervalIsLessThanWaitTime()
    {
        float waitTime = 2.0f;
        var parent = new TestParent();
        var delayer = new Delayer(parent, waitTime);

        var result = delayer.Evaluate(1.0f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, delayer.State);
    }

    [Fact]
    public void Evaluate_ShouldExecuteChildWhenIntervalReachesWaitTime()
    {
        float waitTime = 2.0f;
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var delayer = new Delayer(parent, waitTime);
        delayer.Attach(child);

        var firstTickResult = delayer.Evaluate(1.0f);
        var secondTickResult = delayer.Evaluate(1.0f);

        Assert.Equal(NodeState.Running, firstTickResult);
        Assert.Equal(NodeState.Success, secondTickResult);
        Assert.Equal(NodeState.Success, delayer.State);
        Assert.Equal(1, child.EvaluateCount);
    }

    [Fact]
    public void Evaluate_ShouldResetIntervalWhenChildCompletes()
    {
        float waitTime = 2.0f;
        var parent = new TestParent();
        var child = new TestNode { ReturnState = NodeState.Success };
        var delayer = new Delayer(parent, waitTime);
        delayer.Attach(child);

        delayer.Evaluate(1.0f);
        delayer.Evaluate(1.0f);
        var thirdTickResult = delayer.Evaluate(0.5f);

        Assert.Equal(NodeState.Running, thirdTickResult);
        Assert.Equal(NodeState.Running, delayer.State);
    }

    [Fact]
    public void Abort_ShouldResetIntervalWhenAbortIsCalled()
    {
        float waitTime = 2.0f;
        var parent = new TestParent();
        var child = new TestNode();
        var delayer = new Delayer(parent, waitTime);
        delayer.Attach(child);

        delayer.Evaluate(1.5f);
        delayer.Abort();
        var resultAfterAbort = delayer.Evaluate(0.5f);

        Assert.Equal(NodeState.Running, resultAfterAbort);
        Assert.Equal(NodeState.Running, delayer.State);
        Assert.Equal(0, child.EvaluateCount);
    }

    [Fact]
    public void Reset_ShouldResetIntervalWhenResetIsCalled()
    {
        float waitTime = 2.0f;
        var parent = new TestParent();
        var child = new TestNode();
        var delayer = new Delayer(parent, waitTime);
        delayer.Attach(child);

        delayer.Evaluate(1.5f);
        delayer.Reset();
        var resultAfterReset = delayer.Evaluate(0.5f);

        Assert.Equal(NodeState.Running, resultAfterReset);
        Assert.Equal(NodeState.Running, delayer.State);
        Assert.Equal(0, child.EvaluateCount);
    }
}
