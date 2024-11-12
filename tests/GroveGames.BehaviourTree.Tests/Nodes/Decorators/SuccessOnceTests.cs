using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class SuccessOnceTests
{
    [Fact]
    public void Evaluate_ShouldReturnSuccessOnFirstEvaluation()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChildNode = new Mock<INode>();
        mockChildNode.Setup(x => x.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);
        var executeOnceNode = new SuccessOnce(mockParent.Object);
        executeOnceNode.Attach(mockChildNode.Object);

        // Act
        var resultFirst = executeOnceNode.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, resultFirst);
        Assert.Equal(NodeState.Success, executeOnceNode.State);
    }

    [Fact]
    public void Evaluate_ShouldReturnFailureAfterFirstEvaluation()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChildNode = new Mock<INode>();
        mockChildNode.Setup(x => x.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);
        var executeOnceNode = new SuccessOnce(mockParent.Object);
        executeOnceNode.Attach(mockChildNode.Object);

        // Act
        var resultFirst = executeOnceNode.Evaluate(1.0f);
        var resultSecond = executeOnceNode.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, resultFirst);
        Assert.Equal(NodeState.Failure, resultSecond);
        Assert.Equal(NodeState.Failure, executeOnceNode.State);
    }

    [Fact]
    public void Abort_ShouldAllowReEvaluation()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChildNode = new Mock<INode>();
        mockChildNode.Setup(x => x.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);
        var executeOnceNode = new SuccessOnce(mockParent.Object);
        executeOnceNode.Attach(mockChildNode.Object);

        // Act
        var resultFirst = executeOnceNode.Evaluate(1.0f);
        executeOnceNode.Abort();
        var resultAfterAbort = executeOnceNode.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, resultFirst);
        Assert.Equal(NodeState.Success, resultAfterAbort);
        Assert.Equal(NodeState.Success, executeOnceNode.State);
    }

    [Fact]
    public void Reset_ShouldAllowReEvaluation()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChildNode = new Mock<INode>();
        mockChildNode.Setup(x => x.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);
        var executeOnceNode = new SuccessOnce(mockParent.Object);
        executeOnceNode.Attach(mockChildNode.Object);

        // Act
        var resultFirst = executeOnceNode.Evaluate(1.0f);
        executeOnceNode.Reset();
        var resultAfterReset = executeOnceNode.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, resultFirst);
        Assert.Equal(NodeState.Success, resultAfterReset);
        Assert.Equal(NodeState.Success, executeOnceNode.State);
    }
}