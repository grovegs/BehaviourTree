using System.Reflection;

using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree.Tests.Nodes;

public class NodeTests
{
    public class NodeAccessor
    {
        public static IParent Parent(BehaviourNode node)
        {
            var fieldInfo = typeof(BehaviourNode).GetProperty("Parent", BindingFlags.NonPublic | BindingFlags.Instance);
            return (IParent)fieldInfo?.GetValue(node)!;
        }
    }

    [Fact]
    public void Evaluate_ShouldReturnFailure_ByDefault()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var node = new Mock<BehaviourNode>(mockParent.Object) { CallBase = true };

        // Act
        var result = node.Object.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Failure, result);
    }

    [Fact]
    public void Reset_ShouldNotThrowException()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var node = new Mock<BehaviourNode>(mockParent.Object) { CallBase = true };

        // Act & Assert
        var exception = Record.Exception(node.Object.Reset);
        Assert.Null(exception);
    }

    [Fact]
    public void Abort_ShouldNotThrowException()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var node = new Mock<BehaviourNode>(mockParent.Object) { CallBase = true };

        // Act & Assert
        var exception = Record.Exception(node.Object.Abort);
        Assert.Null(exception);
    }

    [Fact]
    public void Parent_ShouldReturnParent()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var node = new Mock<BehaviourNode>(mockParent.Object) { CallBase = true };

        // Act
        var result = NodeAccessor.Parent(node.Object);

        // Assert
        Assert.Equal(mockParent.Object, result);
    }

    [Fact]
    public void Blackboard_ShouldReturnBlackboardFromParent()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockBlackboard = new Mock<IBlackboard>();
        mockParent.Setup(parent => parent.Blackboard).Returns(mockBlackboard.Object);

        var node = new Mock<BehaviourNode>(mockParent.Object) { CallBase = true };

        // Act
        var result = node.Object.Blackboard;

        // Assert
        Assert.Equal(mockBlackboard.Object, result);
    }
}
