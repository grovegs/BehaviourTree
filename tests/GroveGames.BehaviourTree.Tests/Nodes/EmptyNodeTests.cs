using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree.Tests.Nodes;

public sealed class EmptyNodeTests
{
    [Fact]
    public void Evaluate_ReturnsFailure()
    {
        // Arrange
        var emptyNode = new EmptyBehaviourNode();

        // Act
        var result = emptyNode.Evaluate(0f);

        // Assert
        Assert.Equal(NodeState.Failure, result);
    }

    [Fact]
    public void Reset_DoesNotThrow()
    {
        // Arrange
        var emptyNode = new EmptyBehaviourNode();

        // Act & Assert
        emptyNode.Reset();
    }

    [Fact]
    public void Abort_DoesNotThrow()
    {
        // Arrange
        var emptyNode = new EmptyBehaviourNode();

        // Act & Assert
        emptyNode.Abort();
    }
}
