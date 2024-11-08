using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree.Tests.Nodes.Decorators;

public class DecoratorTests
{
    private class TestDecorator : Decorator
    {
        public TestDecorator(IParent parent) : base(parent) { }
    }

    [Fact]
    public void Attach_ShouldSetChild_WhenChildIsEmpty()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var decorator = new TestDecorator(mockParent.Object);
        var mockNode = new Mock<INode>();

        // Act
        decorator.Attach(mockNode.Object);

        // Assert
        Assert.Throws<ChildAlreadyAttachedException>(() => decorator.Attach(mockNode.Object));
    }

    [Fact]
    public void Attach_ShouldThrowException_WhenChildAlreadyAttached()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var decorator = new TestDecorator(mockParent.Object);
        var mockNode = new Mock<INode>();
        var secondNode = new Mock<INode>();

        // Act
        decorator.Attach(mockNode.Object);

        // Assert
        Assert.Throws<ChildAlreadyAttachedException>(() => decorator.Attach(secondNode.Object));
    }

    [Fact]
    public void Evaluate_ShouldCallEvaluateOnChild()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var decorator = new TestDecorator(mockParent.Object);
        var mockNode = new Mock<INode>();
        decorator.Attach(mockNode.Object);

        // Act
        decorator.Evaluate(0.5f);

        // Assert
        mockNode.Verify(n => n.Evaluate(0.5f), Times.Once);
    }

    [Fact]
    public void Abort_ShouldCallAbortOnChild()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var decorator = new TestDecorator(mockParent.Object);
        var mockNode = new Mock<INode>();
        decorator.Attach(mockNode.Object);

        // Act
        decorator.Abort();

        // Assert
        mockNode.Verify(n => n.Abort(), Times.Once);
    }

    [Fact]
    public void Reset_ShouldCallResetOnChild()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var decorator = new TestDecorator(mockParent.Object);
        var mockNode = new Mock<INode>();
        decorator.Attach(mockNode.Object);

        // Act
        decorator.Reset();

        // Assert
        mockNode.Verify(n => n.Reset(), Times.Once);
    }
}
