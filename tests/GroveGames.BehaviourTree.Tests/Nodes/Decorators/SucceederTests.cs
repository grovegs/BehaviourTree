using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class SucceederTests
{
    [Fact]
    public void Evaluate_WithChildRunning_ShouldReturnRunning()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var succeeder = new Succeeder(mockParent.Object);
        succeeder.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        // Act
        var result = succeeder.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, succeeder.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }

    [Fact]
    public void Evaluate_WithChildSuccess_ShouldReturnSuccess()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var succeeder = new Succeeder(mockParent.Object);
        succeeder.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        // Act
        var result = succeeder.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, succeeder.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }

    [Fact]
    public void Evaluate_WithChildFailure_ShouldReturnSuccess()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var succeeder = new Succeeder(mockParent.Object);
        succeeder.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        // Act
        var result = succeeder.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, succeeder.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }

    [Fact]
    public void Evaluate_ShouldCallChildEvaluateOnce()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var succeeder = new Succeeder(mockParent.Object);
        succeeder.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        // Act
        succeeder.Evaluate(1.0f);

        // Assert
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }
}
