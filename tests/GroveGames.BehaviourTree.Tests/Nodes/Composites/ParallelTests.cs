using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;

using Parallel = GroveGames.BehaviourTree.Nodes.Composites.Parallel;

namespace GroveGames.BehaviourTree.Tests.Nodes.Composites;

public class ParallelTests
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
        public NodeState ReturnState { get; set; } = NodeState.Success;
        public NodeState State => ReturnState;

        public NodeState Evaluate(float deltaTime) => ReturnState;
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
    public void Evaluate_AnySuccessPolicy_ShouldReturnSuccessIfAnyChildSucceeds()
    {
        var parent = new TestParent();
        var parallel = new Parallel(parent, ParallelPolicy.AnySuccess);
        var child1 = new TestNode { ReturnState = NodeState.Failure };
        var child2 = new TestNode { ReturnState = NodeState.Success };

        parallel.Attach(child1).Attach(child2);

        var result = parallel.Evaluate(0.1f);

        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, parallel.State);
    }

    [Fact]
    public void Evaluate_FirstFailurePolicy_ShouldReturnFailureIfAnyChildFails()
    {
        var parent = new TestParent();
        var parallel = new Parallel(parent, ParallelPolicy.FirstFailure);
        var child1 = new TestNode { ReturnState = NodeState.Running };
        var child2 = new TestNode { ReturnState = NodeState.Failure };

        parallel.Attach(child1).Attach(child2);

        var result = parallel.Evaluate(0.1f);

        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, parallel.State);
    }

    [Fact]
    public void Evaluate_AllSuccessPolicy_ShouldReturnSuccessIfAllChildrenSucceed()
    {
        var parent = new TestParent();
        var parallel = new Parallel(parent, ParallelPolicy.AllSuccess);
        var child1 = new TestNode { ReturnState = NodeState.Success };
        var child2 = new TestNode { ReturnState = NodeState.Success };

        parallel.Attach(child1).Attach(child2);

        var result = parallel.Evaluate(0.1f);

        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, parallel.State);
    }

    [Fact]
    public void Evaluate_AllSuccessPolicy_ShouldReturnRunningIfAnyChildIsRunning()
    {
        var parent = new TestParent();
        var parallel = new Parallel(parent, ParallelPolicy.AllSuccess);
        var child1 = new TestNode { ReturnState = NodeState.Running };
        var child2 = new TestNode { ReturnState = NodeState.Success };

        parallel.Attach(child1).Attach(child2);

        var result = parallel.Evaluate(0.1f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, parallel.State);
    }

    [Fact]
    public void Evaluate_AllSuccessPolicy_ShouldReturnFailureIfAllChildrenFail()
    {
        var parent = new TestParent();
        var parallel = new Parallel(parent, ParallelPolicy.AllSuccess);
        var child1 = new TestNode { ReturnState = NodeState.Failure };
        var child2 = new TestNode { ReturnState = NodeState.Failure };

        parallel.Attach(child1).Attach(child2);

        var result = parallel.Evaluate(0.1f);

        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, parallel.State);
    }
}
