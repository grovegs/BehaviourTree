using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;

using Parallel = GroveGames.BehaviourTree.Nodes.Composites.Parallel;

namespace GroveGames.BehaviourTree.Tests;

// Helper classes for testing
public class SuccessNode : Node
{
    public override NodeState Evaluate(IBlackboard blackboard, double delta) => NodeState.SUCCESS;
}

public class FailureNode : Node
{
    public override NodeState Evaluate(IBlackboard blackboard, double delta) => NodeState.FAILURE;
}

public class RunningNode : Node
{
    public override NodeState Evaluate(IBlackboard blackboard, double delta) => NodeState.RUNNING;
}

public class CompositeTests
{
    [Fact]
    public void Composite_AddChild_SetsParent()
    {
        var composite = new Composite();
        var child = new Node();
        composite.AddChild(child);
        var field = typeof(Node).GetField("parent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(child);
        Assert.NotNull(field);
        Assert.Same(composite, field);
    }

    [Fact]
    public void Composite_Interrupt_ResetsProcessingChildIndex()
    {
        var composite = new Composite();
        var child = new SuccessNode();
        composite.AddChild(child);
        composite.Interrupt();
        var field = typeof(Composite).GetField("processingChild", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(composite);
        Assert.NotNull(field);
        Assert.Equal(0, field);
    }
}

public class SelectorTests
{
    [Fact]
    public void Selector_StopsOnSuccess()
    {
        var selector = new Selector();
        selector.AddChild(new FailureNode())
                .AddChild(new SuccessNode())
                .AddChild(new RunningNode());

        var result = selector.Evaluate(null, 0);
        Assert.Equal(NodeState.SUCCESS, result);
    }

    [Fact]
    public void Selector_ReturnsRunningIfChildIsRunning()
    {
        var selector = new Selector();
        selector.AddChild(new FailureNode())
                .AddChild(new RunningNode())
                .AddChild(new SuccessNode());

        var result = selector.Evaluate(null, 0);
        Assert.Equal(NodeState.RUNNING, result);
    }

    [Fact]
    public void Selector_ReturnsFailureIfAllChildrenFail()
    {
        var selector = new Selector();
        selector.AddChild(new FailureNode())
                .AddChild(new FailureNode());

        var result = selector.Evaluate(null, 0);
        Assert.Equal(NodeState.FAILURE, result);
    }
}

public class SequenceTests
{
    [Fact]
    public void Sequence_StopsOnFailure()
    {
        var sequence = new Sequence();
        sequence.AddChild(new SuccessNode())
                .AddChild(new FailureNode())
                .AddChild(new RunningNode());

        var result = sequence.Evaluate(null, 0);
        Assert.Equal(NodeState.FAILURE, result);
    }

    [Fact]
    public void Sequence_ReturnsRunningIfChildIsRunning()
    {
        var sequence = new Sequence();
        sequence.AddChild(new SuccessNode())
                .AddChild(new RunningNode())
                .AddChild(new SuccessNode());

        var result = sequence.Evaluate(null, 0);
        Assert.Equal(NodeState.RUNNING, result);
    }

    [Fact]
    public void Sequence_ReturnsSuccessIfAllChildrenSucceed()
    {
        var sequence = new Sequence();
        sequence.AddChild(new SuccessNode())
                .AddChild(new SuccessNode());

        var result = sequence.Evaluate(null, 0);
        Assert.Equal(NodeState.SUCCESS, result);
    }
}

public class ParallelTests
{
    [Fact]
    public void Parallel_AllSuccessPolicy_ReturnsSuccessIfAllChildrenSucceed()
    {
        var parallel = new Parallel(ParallelPolicy.ALL_SUCCESS);
        parallel.AddChild(new SuccessNode())
                .AddChild(new SuccessNode());

        var result = parallel.Evaluate(null, 0);
        Assert.Equal(NodeState.SUCCESS, result);
    }

    [Fact]
    public void Parallel_AnySuccessPolicy_ReturnsSuccessIfAnyChildSucceeds()
    {
        var parallel = new Parallel(ParallelPolicy.ANY_SUCCESS);
        parallel.AddChild(new FailureNode())
                .AddChild(new SuccessNode())
                .AddChild(new RunningNode());

        var result = parallel.Evaluate(null, 0);
        Assert.Equal(NodeState.SUCCESS, result);
    }

    [Fact]
    public void Parallel_FirstFailurePolicy_ReturnsFailureIfAnyChildFails()
    {
        var parallel = new Parallel(ParallelPolicy.FIRST_FAILURE);
        parallel.AddChild(new SuccessNode())
                .AddChild(new FailureNode())
                .AddChild(new RunningNode());

        var result = parallel.Evaluate(null, 0);
        Assert.Equal(NodeState.FAILURE, result);
    }

    [Fact]
    public void Parallel_ReturnsRunningIfAnyChildIsRunning()
    {
        var parallel = new Parallel(ParallelPolicy.ALL_SUCCESS);
        parallel.AddChild(new SuccessNode())
                .AddChild(new RunningNode())
                .AddChild(new SuccessNode());

        var result = parallel.Evaluate(null, 0);
        Assert.Equal(NodeState.RUNNING, result);
    }
}

