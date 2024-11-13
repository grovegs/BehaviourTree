using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class CooldownTests
{
    [Fact]
    public void Evaluate_ShouldReturnFailureWhenRemainingTimeIsGreaterThanZero()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var cooldown = new Cooldown(mockParent.Object, waitTime);
        cooldown.Attach(mockChild.Object);

        // Act
        cooldown.Evaluate(0.1f);
        var result = cooldown.Evaluate(1f);

        // Assert
        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, cooldown.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }

    [Fact]
    public void Evaluate_ShouldReturnSuccessAfterCooldownExpires()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        mockChild.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);
        var cooldown = new Cooldown(mockParent.Object, waitTime);
        cooldown.Attach(mockChild.Object);

        // Act
        cooldown.Evaluate(0.1f);
        var result = cooldown.Evaluate(2f);

        // Assert
        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, cooldown.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Exactly(2));
    }

    [Fact]
    public void Evaluate_ShouldCallChildEvaluateAfterCooldownExpires()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var cooldown = new Cooldown(mockParent.Object, waitTime);
        cooldown.Attach(mockChild.Object);
        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        // Act
        cooldown.Evaluate(0.1f);
        var result = cooldown.Evaluate(2f);

        // Assert
        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, cooldown.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Exactly(2));
    }

    [Fact]
    public void Reset_ShouldResetRemainingTimeToZero()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        mockChild.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);
        var cooldown = new Cooldown(mockParent.Object, waitTime);
        cooldown.Attach(mockChild.Object);

        // Act
        cooldown.Evaluate(1.0f);
        cooldown.Reset();
        var resultAfterReset = cooldown.Evaluate(0.5f);

        // Assert
        Assert.Equal(NodeState.Success, resultAfterReset);
        Assert.Equal(NodeState.Success, cooldown.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Exactly(2));
    }

    [Fact]
    public void Abort_ShouldResetRemainingTimeToZero()
    {
        // Arrange
        float waitTime = 2.0f;
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        mockChild.Setup(c => c.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);
        var cooldown = new Cooldown(mockParent.Object, waitTime);
        cooldown.Attach(mockChild.Object);

        // Act
        cooldown.Evaluate(1.0f);
        cooldown.Abort();
        var resultAfterAbort = cooldown.Evaluate(0.5f);

        // Assert
        Assert.Equal(NodeState.Success, resultAfterAbort);
        Assert.Equal(NodeState.Success, cooldown.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Exactly(2));
    }
}
