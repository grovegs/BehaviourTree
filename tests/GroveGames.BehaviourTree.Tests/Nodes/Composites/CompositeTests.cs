using System.Reflection;
using System.Runtime.CompilerServices;

using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;

namespace GroveGames.BehaviourTree.Tests.Nodes.Composites;

public class CompositeTests
{
    public class TestComposite : Composite
    {
        public TestComposite(IParent parent) : base(parent) { }
    }

    public class CompositeAccessor
    {
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "Children")]
        public static IReadOnlyList<INode> Children(Composite composite)
        {
            var fieldInfo = typeof(Selector).GetProperty("Children", BindingFlags.NonPublic | BindingFlags.Instance);
            return (IReadOnlyList<INode>)fieldInfo?.GetValue(composite)!;
        }
    }

    [Fact]
    public static void Attach_AddsNodeToChildren()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockNode = new Mock<INode>();
        var composite = new TestComposite(mockParent.Object);

        // Act
        composite.Attach(mockNode.Object);

        // Assert
        Assert.Contains(mockNode.Object, CompositeAccessor.Children(composite));
    }

    [Fact]
    public static void Reset_CallsResetOnAllChildren()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockChild1 = new Mock<INode>();
        var mockChild2 = new Mock<INode>();
        var composite = new TestComposite(mockParent.Object);

        composite.Attach(mockChild1.Object);
        composite.Attach(mockChild2.Object);

        // Act
        composite.Reset();

        // Assert
        mockChild1.Verify(child => child.Reset(), Times.Once);
        mockChild2.Verify(child => child.Reset(), Times.Once);
    }
}
