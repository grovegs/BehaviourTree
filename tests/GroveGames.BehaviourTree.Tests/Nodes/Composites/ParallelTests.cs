using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;

using Parallel = GroveGames.BehaviourTree.Nodes.Composites.Parallel;

namespace GroveGames.BehaviourTree.Tests.Nodes.Composites;

public class ParallelTests
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
        public NodeState ReturnState { get; set; } = NodeState.Success;
        public NodeState State => ReturnState;

        public string Name => string.Empty;

        public NodeState Evaluate(float deltaTime) => ReturnState;
        public void Reset() { }
        public int AbortCount { get; private set; }
        public void Abort() => AbortCount++;
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
    public void Evaluate_AnySuccessPolicy_ShouldReturnSuccessIfAnyChildSucceeds()
    {
        var parent = new FakeParent();
        var parallel = new Parallel(ParallelPolicy.AnySuccess);
        var child1 = new FakeNode { ReturnState = NodeState.Failure };
        var child2 = new FakeNode { ReturnState = NodeState.Success };

        parallel.Attach(child1).Attach(child2);

        var result = parallel.Evaluate(0.1f);

        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, parallel.State);
    }

    [Fact]
    public void Evaluate_FirstFailurePolicy_ShouldReturnFailureIfAnyChildFails()
    {
        var parent = new FakeParent();
        var parallel = new Parallel(ParallelPolicy.FirstFailure);
        var child1 = new FakeNode { ReturnState = NodeState.Running };
        var child2 = new FakeNode { ReturnState = NodeState.Failure };

        parallel.Attach(child1).Attach(child2);

        var result = parallel.Evaluate(0.1f);

        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, parallel.State);
    }

    [Fact]
    public void Evaluate_AllSuccessPolicy_ShouldReturnSuccessIfAllChildrenSucceed()
    {
        var parent = new FakeParent();
        var parallel = new Parallel(ParallelPolicy.AllSuccess);
        var child1 = new FakeNode { ReturnState = NodeState.Success };
        var child2 = new FakeNode { ReturnState = NodeState.Success };

        parallel.Attach(child1).Attach(child2);

        var result = parallel.Evaluate(0.1f);

        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, parallel.State);
    }

    [Fact]
    public void Evaluate_AllSuccessPolicy_ShouldReturnRunningIfAnyChildIsRunning()
    {
        var parent = new FakeParent();
        var parallel = new Parallel(ParallelPolicy.AllSuccess);
        var child1 = new FakeNode { ReturnState = NodeState.Running };
        var child2 = new FakeNode { ReturnState = NodeState.Success };

        parallel.Attach(child1).Attach(child2);

        var result = parallel.Evaluate(0.1f);

        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, parallel.State);
    }

    [Fact]
    public void Evaluate_AllSuccessPolicy_ShouldReturnFailureIfAllChildrenFail()
    {
        var parent = new FakeParent();
        var parallel = new Parallel(ParallelPolicy.AllSuccess);
        var child1 = new FakeNode { ReturnState = NodeState.Failure };
        var child2 = new FakeNode { ReturnState = NodeState.Failure };

        parallel.Attach(child1).Attach(child2);

        var result = parallel.Evaluate(0.1f);

        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, parallel.State);
    }

    [Fact]
    public void Evaluate_AnySuccessResolved_AbortsRunningSiblings()
    {
        var parallel = new Parallel(ParallelPolicy.AnySuccess);
        var running = new FakeNode { ReturnState = NodeState.Running };
        var succeeding = new FakeNode { ReturnState = NodeState.Success };
        parallel.Attach(running);
        parallel.Attach(succeeding);

        parallel.Evaluate(0.1f);

        Assert.Equal(1, running.AbortCount);
        Assert.Equal(0, succeeding.AbortCount);
    }

    [Fact]
    public void Abort_AbortsEveryChild()
    {
        var parallel = new Parallel(ParallelPolicy.AllSuccess);
        var first = new FakeNode { ReturnState = NodeState.Running };
        var second = new FakeNode { ReturnState = NodeState.Running };
        parallel.Attach(first);
        parallel.Attach(second);

        parallel.Abort();

        Assert.Equal(1, first.AbortCount);
        Assert.Equal(1, second.AbortCount);
    }
}
