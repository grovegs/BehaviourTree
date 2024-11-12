using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class AbortTests
{
    [Fact]
    public void Evaluate_ShouldReturnSuccessWhenConditionIsTrue()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var abortCondition = new Mock<Func<bool>>();
        abortCondition.Setup(c => c()).Returns(true);

        var abort = new Abort(mockParent.Object, abortCondition.Object);
        abort.Attach(mockChild.Object);

        // Act
        var result = abort.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, abort.State);
        mockChild.Verify(child => child.Abort(), Times.Once);
    }

    [Fact]
    public void Evaluate_ShouldCallChildEvaluateWhenConditionIsFalse()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var abortCondition = new Mock<Func<bool>>();
        abortCondition.Setup(c => c()).Returns(false);

        var abort = new Abort(mockParent.Object, abortCondition.Object);
        abort.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        // Act
        var result = abort.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, abort.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }

    [Fact]
    public void Evaluate_ShouldNotCallChildEvaluateWhenConditionIsTrue()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var abortCondition = new Mock<Func<bool>>();
        abortCondition.Setup(c => c()).Returns(true);

        var abort = new Abort(mockParent.Object, abortCondition.Object);
        abort.Attach(mockChild.Object);

        // Act
        var result = abort.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, result);
        Assert.Equal(NodeState.Success, abort.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Never);
    }

    [Fact]
    public void Abort_ShouldCallChildAbortWhenConditionIsTrue()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var abortCondition = new Mock<Func<bool>>();
        abortCondition.Setup(c => c()).Returns(true);

        var abort = new Abort(mockParent.Object, abortCondition.Object);
        abort.Attach(mockChild.Object);

        // Act
        abort.Evaluate(1.0f);

        // Assert
        mockChild.Verify(child => child.Abort(), Times.Once);
    }

    [Fact]
    public void Abort_ShouldNotCallChildAbortWhenConditionIsFalse()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var abortCondition = new Mock<Func<bool>>();
        abortCondition.Setup(c => c()).Returns(false);

        var abort = new Abort(mockParent.Object, abortCondition.Object);
        abort.Attach(mockChild.Object);

        // Act
        abort.Evaluate(1.0f);

        // Assert
        mockChild.Verify(child => child.Abort(), Times.Never);
    }
}
