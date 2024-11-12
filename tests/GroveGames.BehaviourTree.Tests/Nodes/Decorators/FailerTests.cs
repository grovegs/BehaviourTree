using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class FailerTests
{
    [Fact]
    public void Evaluate_ShouldReturnFailureWhenChildReturnsSuccess()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var failer = new Failer(mockParent.Object);
        failer.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        // Act
        var result = failer.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, failer.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }

    [Fact]
    public void Evaluate_ShouldReturnFailureWhenChildReturnsFailure()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var failer = new Failer(mockParent.Object);
        failer.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        // Act
        var result = failer.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, failer.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }

    [Fact]
    public void Evaluate_ShouldReturnRunningWhenChildReturnsRunning()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var failer = new Failer(mockParent.Object);
        failer.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        // Act
        var result = failer.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, failer.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }
}
