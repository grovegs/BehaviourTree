using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;

namespace GroveGames.BehaviourTree.Tests.Nodes.Composites;

public class SequenceTests
{
    [Fact]
    public void Evaluate_ShouldReturnRunningWhenFirstChildIsRunning()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var sequence = new Sequence(mockParent.Object);
        sequence.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        // Act
        var result = sequence.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, sequence.State);
    }

    [Fact]
    public void Evaluate_ShouldReturnFailureWhenChildFails()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var sequence = new Sequence(mockParent.Object);
        sequence.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        // Act
        var result = sequence.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, sequence.State);
    }

    [Fact]
    public void Evaluate_ShouldReturnSuccessWhenAllChildrenSucceed()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild1 = new Mock<INode>();
        var mockChild2 = new Mock<INode>();
        var sequence = new Sequence(mockParent.Object);
        sequence.Attach(mockChild1.Object);
        sequence.Attach(mockChild2.Object);

        mockChild1.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);
        mockChild2.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        // Act
        sequence.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, sequence.State);
    }

    [Fact]
    public void Evaluate_ShouldResetOnFailureAfterSuccess()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild1 = new Mock<INode>();
        var mockChild2 = new Mock<INode>();
        var sequence = new Sequence(mockParent.Object);
        sequence.Attach(mockChild1.Object);
        sequence.Attach(mockChild2.Object);

        mockChild1.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);
        mockChild2.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        // Act
        sequence.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Failure, sequence.State);
    }

    [Fact]
    public void Reset_ShouldResetProcessingChildIndex()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild1 = new Mock<INode>();
        var mockChild2 = new Mock<INode>();
        var sequence = new Sequence(mockParent.Object);
        sequence.Attach(mockChild1.Object);
        sequence.Attach(mockChild2.Object);

        mockChild1.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);
        mockChild2.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        sequence.Evaluate(1.0f);

        // Act
        sequence.Reset();
        var resetResult = sequence.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, sequence.State);
    }

    [Fact]
    public void Abort_ShouldResetProcessingChildIndex()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild1 = new Mock<INode>();
        var mockChild2 = new Mock<INode>();
        var sequence = new Sequence(mockParent.Object);
        sequence.Attach(mockChild1.Object);
        sequence.Attach(mockChild2.Object);

        mockChild1.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);
        mockChild2.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        sequence.Evaluate(1.0f);

        // Act
        sequence.Abort();
        var resultAfterAbort = sequence.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, sequence.State);
    }
}
