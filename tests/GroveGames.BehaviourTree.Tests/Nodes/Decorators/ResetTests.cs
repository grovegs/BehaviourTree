using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class ResetTests
{
    [Fact]
    public void Evaluate_WithConditionTrue_ShouldCallResetAndReturnSuccess()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        bool condition = true;
        var reset = new Reset(mockParent.Object, () => condition);
        reset.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        // Act
        var result = reset.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, result);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
        mockChild.Verify(child => child.Reset(), Times.Once);
    }

    [Fact]
    public void Evaluate_WithConditionFalse_ShouldNotCallResetAndReturnChildState()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        bool condition = false;
        var reset = new Reset(mockParent.Object, () => condition);
        reset.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        // Act
        var result = reset.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Running, result);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
        mockChild.Verify(child => child.Reset(), Times.Never);
    }

    [Fact]
    public void Evaluate_WithConditionTrue_ShouldResetChildWhenCalledMultipleTimes()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        bool condition = true;
        var reset = new Reset(mockParent.Object, () => condition);
        reset.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        // Act
        var firstEvaluation = reset.Evaluate(1.0f);
        var secondEvaluation = reset.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, firstEvaluation);
        Assert.Equal(NodeState.Success, secondEvaluation);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Exactly(2));
        mockChild.Verify(child => child.Reset(), Times.Exactly(2));
    }

    [Fact]
    public void Reset_ShouldNotCallChildResetWhenConditionIsFalse()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        bool condition = false;
        var reset = new Reset(mockParent.Object, () => condition);
        reset.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        // Act
        var result = reset.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Running, result);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
        mockChild.Verify(child => child.Reset(), Times.Never);
    }
}
