using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Composites;

public class DelayerTests
{
    [Fact]
    public void Delayer_ReturnsRunning_WhenIntervalIsLessThanWaitTime()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var delayer = new Delayer(mockParent.Object, waitTime);

        // Act
        var result = delayer.Evaluate(1.0f); // First tick with a deltaTime less than the wait time

        // Assert
        Assert.Equal(NodeState.Running, result);
    }

    [Fact]
    public void Delayer_ExecutesChild_WhenIntervalReachesWaitTime()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var delayer = new Delayer(mockParent.Object, waitTime);
        delayer.Attach(mockChild.Object); // Attach the child node to the delayer

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success); // Set up the child node to return SUCCESS when evaluated

        // Act
        var firstTickResult = delayer.Evaluate(1.0f); // First tick (still running as wait time has not been reached)
        var secondTickResult = delayer.Evaluate(1.0f); // Second tick (reaches wait time)

        // Assert
        Assert.Equal(NodeState.Running, firstTickResult);
        Assert.Equal(NodeState.Success, secondTickResult);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once); // Child should be evaluated once
    }

    [Fact]
    public void Delayer_ResetsInterval_WhenChildCompletes()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var delayer = new Delayer(mockParent.Object, waitTime);
        delayer.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

        // Act
        delayer.Evaluate(1.0f); // First tick (still running as wait time has not been reached)
        delayer.Evaluate(1.0f); // Second tick (reaches wait time, evaluates child, and should reset interval)
        var thirdTickResult = delayer.Evaluate(0.5f); // Next tick should be running as the interval has reset

        // Assert
        Assert.Equal(NodeState.Running, thirdTickResult);
    }

    [Fact]
    public void Delayer_Abort_ResetsInterval()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var delayer = new Delayer(mockParent.Object, waitTime);
        delayer.Attach(mockChild.Object);

        delayer.Evaluate(1.5f); // Increment the interval by evaluating once

        // Act
        delayer.Abort(); // Call Abort and check if interval has reset
        var resultAfterAbort = delayer.Evaluate(0.5f);

        // Assert
        Assert.Equal(NodeState.Running, resultAfterAbort);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Never);
    }

    [Fact]
    public void Delayer_Reset_ResetsInterval()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var delayer = new Delayer(mockParent.Object, waitTime);
        delayer.Attach(mockChild.Object);

        delayer.Evaluate(1.5f); // Increment the interval by evaluating once

        // Act
        delayer.Reset(); // Call Reset and check if interval has reset
        var resultAfterReset = delayer.Evaluate(0.5f);

        // Assert
        Assert.Equal(NodeState.Running, resultAfterReset);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Never);
    }
}
