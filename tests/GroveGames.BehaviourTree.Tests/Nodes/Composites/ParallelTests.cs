using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;

using Parallel = GroveGames.BehaviourTree.Nodes.Composites.Parallel;

namespace GroveGames.BehaviourTree.Tests.Nodes.Composites;

public class ParallelTests
{
    [Fact]
    public void Evaluate_AnySuccessPolicy_ShouldReturnSuccessIfAnyChildSucceeds()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var parallel = new Parallel(mockParent.Object, ParallelPolicy.AnySuccess);

        var child1 = new Mock<INode>();
        child1.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        var child2 = new Mock<INode>();
        child2.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        parallel.Attach(child1.Object).Attach(child2.Object);

        // Act
        var result = parallel.Evaluate(0.1f);

        // Assert
        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, parallel.State);
    }

    [Fact]
    public void Evaluate_FirstFailurePolicy_ShouldReturnFailureIfAnyChildFails()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var parallel = new Parallel(mockParent.Object, ParallelPolicy.FirstFailure);

        var child1 = new Mock<INode>();
        child1.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        var child2 = new Mock<INode>();
        child2.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        parallel.Attach(child1.Object).Attach(child2.Object);

        // Act
        var result = parallel.Evaluate(0.1f);

        // Assert
        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, parallel.State);
    }

    [Fact]
    public void Evaluate_AllSuccessPolicy_ShouldReturnSuccessIfAllChildrenSucceed()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var parallel = new Parallel(mockParent.Object, ParallelPolicy.AllSuccess);

        var child1 = new Mock<INode>();
        child1.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        var child2 = new Mock<INode>();
        child2.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        parallel.Attach(child1.Object).Attach(child2.Object);

        // Act
        var result = parallel.Evaluate(0.1f);

        // Assert
        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, parallel.State);
    }

    [Fact]
    public void Evaluate_AllSuccessPolicy_ShouldReturnRunningIfAnyChildIsRunning()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var parallel = new Parallel(mockParent.Object, ParallelPolicy.AllSuccess);

        var child1 = new Mock<INode>();
        child1.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        var child2 = new Mock<INode>();
        child2.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        parallel.Attach(child1.Object).Attach(child2.Object);

        // Act
        var result = parallel.Evaluate(0.1f);

        // Assert
        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, parallel.State);
    }

    [Fact]
    public void Evaluate_AllSuccessPolicy_ShouldReturnFailureIfAllChildrenFail()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var parallel = new Parallel(mockParent.Object, ParallelPolicy.AllSuccess);

        var child1 = new Mock<INode>();
        child1.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        var child2 = new Mock<INode>();
        child2.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        parallel.Attach(child1.Object).Attach(child2.Object);

        // Act
        var result = parallel.Evaluate(0.1f);

        // Assert
        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, parallel.State);
    }
}