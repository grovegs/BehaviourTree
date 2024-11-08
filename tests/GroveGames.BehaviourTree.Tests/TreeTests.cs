using System.Reflection;

using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree.Tests;

public class TreeTests
{
    public class TreeAccessor
    {
        public static bool IsEnabled(Tree tree)
        {
            var fieldInfo = typeof(Tree).GetField("_isEnabled", BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)fieldInfo?.GetValue(tree)!;
        }
    }

    public class TestTree : Tree
    {
        public TestTree(IRoot root) : base(root)
        {
        }

        public override void SetupTree()
        {

        }
    }

    [Fact]
    public void Tick_ShouldNotEvaluate_WhenTreeIsDisabled()
    {
        // Arrange
        var mockRoot = new Mock<IRoot>();
        var tree = new TestTree(mockRoot.Object);

        // Act
        tree.Tick(1.0f);

        // Assert
        mockRoot.Verify(root => root.Evaluate(It.IsAny<float>()), Times.Never);
    }

    [Fact]
    public void Tick_ShouldEvaluate_WhenTreeIsEnabled()
    {
        // Arrange
        var mockRoot = new Mock<IRoot>();
        var tree = new TestTree(mockRoot.Object);
        tree.Enable();

        // Act
        tree.Tick(1.0f);

        // Assert
        mockRoot.Verify(root => root.Evaluate(It.IsAny<float>()), Times.Once);
    }

    [Fact]
    public void Reset_ShouldCallResetOnRoot()
    {
        // Arrange
        var mockRoot = new Mock<IRoot>();
        var tree = new TestTree(mockRoot.Object);

        // Act
        tree.Reset();

        // Assert
        mockRoot.Verify(root => root.Reset(), Times.Once);
    }

    [Fact]
    public void Abort_ShouldCallAbortOnRoot()
    {
        // Arrange
        var mockRoot = new Mock<IRoot>();
        var tree = new TestTree(mockRoot.Object);

        // Act
        tree.Abort();

        // Assert
        mockRoot.Verify(root => root.Abort(), Times.Once);
    }

    [Fact]
    public void Enable_ShouldSetIsEnabledToTrue()
    {
        // Arrange
        var mockRoot = new Mock<IRoot>();
        var tree = new TestTree(mockRoot.Object);

        // Act
        tree.Enable();

        // Assert
        Assert.True(TreeAccessor.IsEnabled(tree));
    }

    [Fact]
    public void Disable_ShouldSetIsEnabledToFalse()
    {
        // Arrange
        var mockRoot = new Mock<IRoot>();
        var tree = new TestTree(mockRoot.Object);

        // Act
        tree.Disable();

        // Assert
        Assert.False(TreeAccessor.IsEnabled(tree));
    }
}