using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class DelayerTests
{
    [Fact]
    public void Evaluate_ShouldReturnRunningWhenIntervalIsLessThanWaitTime()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var delayer = new Delayer(mockParent.Object, waitTime);

        // Act
        var result = delayer.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, delayer.State);
    }

    [Fact]
    public void Evaluate_ShouldExecuteChildWhenIntervalReachesWaitTime()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var delayer = new Delayer(mockParent.Object, waitTime);
        delayer.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        // Act
        var firstTickResult = delayer.Evaluate(1.0f);
        var secondTickResult = delayer.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Running, firstTickResult);
        Assert.Equal(NodeState.Success, secondTickResult);
        Assert.Equal(NodeState.Success, delayer.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }

    [Fact]
    public void Evaluate_ShouldResetIntervalWhenChildCompletes()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var delayer = new Delayer(mockParent.Object, waitTime);
        delayer.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        // Act
        delayer.Evaluate(1.0f);
        delayer.Evaluate(1.0f);
        var thirdTickResult = delayer.Evaluate(0.5f);

        // Assert
        Assert.Equal(NodeState.Running, thirdTickResult);
        Assert.Equal(NodeState.Running, delayer.State);
    }

    [Fact]
    public void Abort_ShouldResetIntervalWhenAbortIsCalled()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var delayer = new Delayer(mockParent.Object, waitTime);
        delayer.Attach(mockChild.Object);

        delayer.Evaluate(1.5f);

        // Act
        delayer.Abort();
        var resultAfterAbort = delayer.Evaluate(0.5f);

        // Assert
        Assert.Equal(NodeState.Running, resultAfterAbort);
        Assert.Equal(NodeState.Running, delayer.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Never);
    }

    [Fact]
    public void Reset_ShouldResetIntervalWhenResetIsCalled()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var delayer = new Delayer(mockParent.Object, waitTime);
        delayer.Attach(mockChild.Object);

        delayer.Evaluate(1.5f);

        // Act
        delayer.Reset();
        var resultAfterReset = delayer.Evaluate(0.5f);

        // Assert
        Assert.Equal(NodeState.Running, resultAfterReset);
        Assert.Equal(NodeState.Running, delayer.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Never);
    }
}