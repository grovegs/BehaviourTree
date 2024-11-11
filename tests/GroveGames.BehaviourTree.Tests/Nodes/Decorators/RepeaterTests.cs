using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class RepeaterTests
{
    [Fact]
    public void Evaluate_WithFixedCount_ShouldReturnRunningUntilMaxCountReached()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var repeater = new Repeater(mockParent.Object, RepeatMode.FixedCount, 2);
        repeater.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        // Act
        var firstEvaluation = repeater.Evaluate(1.0f);
        var secondEvaluation = repeater.Evaluate(1.0f);
        var thirdEvaluation = repeater.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Running, firstEvaluation);
        Assert.Equal(NodeState.Running, secondEvaluation);
        Assert.Equal(NodeState.Success, thirdEvaluation);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Exactly(2));
    }

    [Fact]
    public void Evaluate_WithUntilSuccess_ShouldReturnSuccessWhenChildSucceeds()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var repeater = new Repeater(mockParent.Object, RepeatMode.UntilSuccess);
        repeater.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        // Act
        var result = repeater.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, result);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }

    [Fact]
    public void Evaluate_WithUntilFailure_ShouldReturnFailureWhenChildFails()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var repeater = new Repeater(mockParent.Object, RepeatMode.UntilFailure);
        repeater.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Failure);

        // Act
        var result = repeater.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Failure, result);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }

    [Fact]
    public void Evaluate_WithInfiniteRepeat_ShouldAlwaysReturnRunning()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var repeater = new Repeater(mockParent.Object, RepeatMode.Infinite);
        repeater.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        // Act
        var firstEvaluation = repeater.Evaluate(1.0f);
        var secondEvaluation = repeater.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Running, firstEvaluation);
        Assert.Equal(NodeState.Running, secondEvaluation);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Exactly(2));
    }

    [Fact]
    public void Evaluate_WithReset_ShouldResetCurrentCount()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var repeater = new Repeater(mockParent.Object, RepeatMode.FixedCount, 2);
        repeater.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        repeater.Evaluate(1.0f);
        repeater.Evaluate(1.0f);

        // Act
        repeater.Reset();
        var resultAfterReset = repeater.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Running, resultAfterReset);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Exactly(3));
    }

    [Fact]
    public void Abort_ShouldResetCurrentCount()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var repeater = new Repeater(mockParent.Object, RepeatMode.FixedCount, 2);
        repeater.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        repeater.Evaluate(1.0f);
        repeater.Evaluate(1.0f);

        // Act
        repeater.Abort();
        var resultAfterAbort = repeater.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Running, resultAfterAbort);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Exactly(3));
    }
}
