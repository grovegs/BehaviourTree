using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class InverterTests
{
    [Fact]
    public void Evaluate_ShouldReturnSuccessWhenChildReturnsFailure()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var inverter = new Inverter(mockParent.Object);
        inverter.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        // Act
        var result = inverter.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, inverter.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }

    [Fact]
    public void Evaluate_ShouldReturnFailureWhenChildReturnsSuccess()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var inverter = new Inverter(mockParent.Object);
        inverter.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        // Act
        var result = inverter.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, inverter.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }

    [Fact]
    public void Evaluate_ShouldReturnRunningWhenChildReturnsRunning()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var inverter = new Inverter(mockParent.Object);
        inverter.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        // Act
        var result = inverter.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, inverter.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }
}
