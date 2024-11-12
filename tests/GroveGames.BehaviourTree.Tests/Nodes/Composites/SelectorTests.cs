using System.Reflection;

using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;

namespace GroveGames.BehaviourTree.Tests.Nodes.Composites;

public class SelectorTests
{
    public class SelectorAccessor
    {
        public static int GetProcessingChildIndex(Selector selector)
        {
            var fieldInfo = typeof(Selector).GetField("_processingChildIndex", BindingFlags.NonPublic | BindingFlags.Instance);
            return (int)fieldInfo?.GetValue(selector)!;
        }
    }

    [Fact]
    public static void Evaluate_ReturnsRunningAndMovesToNextChildOnFailure()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var selector = new Selector(mockParent.Object);

        var child1 = new Mock<INode>();
        child1.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        var child2 = new Mock<INode>();
        child2.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        selector.Attach(child1.Object).Attach(child2.Object);

        // Act
        var result = selector.Evaluate(0.1f);

        // Assert
        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, selector.State);
        Assert.Equal(1, SelectorAccessor.GetProcessingChildIndex(selector));
    }

    [Fact]
    public static void Evaluate_ReturnsRunningIfCurrentChildIsRunning()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var selector = new Selector(mockParent.Object);

        var child = new Mock<INode>();
        child.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        selector.Attach(child.Object);

        // Act
        var result = selector.Evaluate(0.1f);

        // Assert
        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, selector.State);
        Assert.Equal(0, SelectorAccessor.GetProcessingChildIndex(selector));
    }

    [Fact]
    public static void Evaluate_ReturnsSuccessAndResetsIfAnyChildSucceeds()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var selector = new Selector(mockParent.Object);

        var child1 = new Mock<INode>();
        child1.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        var child2 = new Mock<INode>();
        child2.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        selector.Attach(child1.Object).Attach(child2.Object);

        // Act
        selector.Evaluate(0.1f);
        var result = selector.Evaluate(0.1f);

        // Assert
        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, selector.State);
        Assert.Equal(0, SelectorAccessor.GetProcessingChildIndex(selector));
    }

    [Fact]
    public static void Evaluate_ReturnsFailureIfAllChildrenFail()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var selector = new Selector(mockParent.Object);

        var child1 = new Mock<INode>();
        child1.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        var child2 = new Mock<INode>();
        child2.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        selector.Attach(child1.Object).Attach(child2.Object);

        // Act
        selector.Evaluate(0.1f);
        selector.Evaluate(0.1f);
        var result = selector.Evaluate(0.1f);

        // Assert
        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, selector.State);
        Assert.Equal(0, SelectorAccessor.GetProcessingChildIndex(selector));
    }

    [Fact]
    public static void Reset_ResetsProcessingChildIndex()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var selector = new Selector(mockParent.Object);

        var child = new Mock<INode>();
        child.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        selector.Attach(child.Object);
        selector.Evaluate(0.1f);

        // Act
        selector.Reset();

        // Assert
        Assert.Equal(0, SelectorAccessor.GetProcessingChildIndex(selector));
    }

    [Fact]
    public static void Abort_CallsAbortOnCurrentChildAndResetsProcessingChildIndex()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var selector = new Selector(mockParent.Object);

        var child = new Mock<INode>();
        child.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        selector.Attach(child.Object);
        selector.Evaluate(0.1f);

        // Act
        selector.Abort();

        // Assert
        child.Verify(c => c.Abort(), Times.Once);
        Assert.Equal(0, SelectorAccessor.GetProcessingChildIndex(selector));
    }
}
