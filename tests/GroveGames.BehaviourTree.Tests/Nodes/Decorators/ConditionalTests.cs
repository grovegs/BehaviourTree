using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class ConditionalTests
{
    [Fact]
    public void Evaluate_ShouldReturnFailureWhenConditionIsFalse()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var condition = new Mock<Func<bool>>();
        condition.Setup(c => c()).Returns(false);

        var conditional = new Conditonal(mockParent.Object, condition.Object);
        conditional.Attach(mockChild.Object);

        // Act
        var result = conditional.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, conditional.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Never);
    }

    [Fact]
    public void Evaluate_ShouldCallChildEvaluateWhenConditionIsTrue()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var condition = new Mock<Func<bool>>();
        condition.Setup(c => c()).Returns(true);

        var conditional = new Conditonal(mockParent.Object, condition.Object);
        conditional.Attach(mockChild.Object);

        mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Running);

        // Act
        var result = conditional.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Running, result);
        Assert.Equal(NodeState.Running, conditional.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once);
    }

    [Fact]
    public void Evaluate_ShouldNotCallChildEvaluateWhenConditionIsFalse()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild = new Mock<INode>();
        var condition = new Mock<Func<bool>>();
        condition.Setup(c => c()).Returns(false);

        var conditional = new Conditonal(mockParent.Object, condition.Object);
        conditional.Attach(mockChild.Object);

        // Act
        var result = conditional.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Failure, result);
        Assert.Equal(NodeState.Failure, conditional.State);
        mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Never);
    }
}
