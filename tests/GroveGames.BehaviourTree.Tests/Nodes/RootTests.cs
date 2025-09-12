using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree.Tests.Nodes;

public class RootTests
{
    [Fact]
    public void Constructor_ShouldInitializeBlackboard()
    {
        // Arrange
        var mockBlackboard = new Mock<IBlackboard>();

        // Act
        var root = new BehaviourRoot(mockBlackboard.Object);

        // Assert
        Assert.Equal(mockBlackboard.Object, root.Blackboard);
    }

    [Fact]
    public void Attach_ShouldSetChild_WhenChildIsEmpty()
    {
        // Arrange
        var mockBlackboard = new Mock<IBlackboard>();
        var root = new BehaviourRoot(mockBlackboard.Object);
        var mockNode = new Mock<INode>();

        // Act
        root.Attach(mockNode.Object);

        // Assert
        Assert.Throws<ChildAlreadyAttachedException>(() => root.Attach(mockNode.Object));
    }

    [Fact]
    public void Attach_ShouldThrowException_WhenChildAlreadyAttached()
    {
        // Arrange
        var mockBlackboard = new Mock<IBlackboard>();
        var root = new BehaviourRoot(mockBlackboard.Object);
        var mockNode = new Mock<INode>();
        var secondNode = new Mock<INode>();

        // Act
        root.Attach(mockNode.Object);

        // Assert
        Assert.Throws<ChildAlreadyAttachedException>(() => root.Attach(secondNode.Object));
    }

    [Fact]
    public void Evaluate_ShouldCallEvaluateOnChild()
    {
        // Arrange
        var mockBlackboard = new Mock<IBlackboard>();
        var root = new BehaviourRoot(mockBlackboard.Object);
        var mockNode = new Mock<INode>();
        root.Attach(mockNode.Object);

        // Act
        root.Evaluate(0.5f);

        // Assert
        mockNode.Verify(n => n.Evaluate(0.5f), Times.Once);
    }

    [Fact]
    public void Abort_ShouldCallAbortOnChild()
    {
        // Arrange
        var mockBlackboard = new Mock<IBlackboard>();
        var root = new BehaviourRoot(mockBlackboard.Object);
        var mockNode = new Mock<INode>();
        root.Attach(mockNode.Object);

        // Act
        root.Abort();

        // Assert
        mockNode.Verify(n => n.Abort(), Times.Once);
    }

    [Fact]
    public void Reset_ShouldCallResetOnChild()
    {
        // Arrange
        var mockBlackboard = new Mock<IBlackboard>();
        var root = new BehaviourRoot(mockBlackboard.Object);
        var mockNode = new Mock<INode>();
        root.Attach(mockNode.Object);

        // Act
        root.Reset();

        // Assert
        mockNode.Verify(n => n.Reset(), Times.Once);
    }
}
