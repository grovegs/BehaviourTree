using System.Reflection;

using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree.Tests;

public class TreeTests
{
    private sealed class TreeAccessor
    {
        public static bool IsEnabled(BehaviourTree tree)
        {
            var fieldInfo = typeof(BehaviourTree).GetField("_isEnabled", BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)fieldInfo?.GetValue(tree)!;
        }
    }

    private sealed class TestTree : BehaviourTree
    {
        public TestTree(IRoot root) : base(root)
        {
        }

        public override void SetupTree()
        {
        }
    }

    private sealed class TestRoot : IRoot
    {
        public int EvaluateCallCount { get; private set; }
        public int ResetCallCount { get; private set; }
        public int AbortCallCount { get; private set; }
        public NodeState State { get; private set; } = NodeState.None;
        public IBlackboard Blackboard => throw new NotImplementedException();

        public NodeState Evaluate(float deltaTime)
        {
            EvaluateCallCount++;
            return State;
        }

        public void Reset()
        {
            ResetCallCount++;
        }

        public void Abort()
        {
            AbortCallCount++;
        }

        public void StartEvaluate()
        {
        }

        public void EndEvaluate()
        {
        }

        public IParent Attach(INode node)
        {
            return this;
        }

        public IParent Attach(IChildTree tree)
        {
            return this;
        }

        public void SetParent(IParent parent)
        {
        }
    }

    [Fact]
    public void Tick_ShouldNotEvaluate_WhenTreeIsDisabled()
    {
        var root = new TestRoot();
        var tree = new TestTree(root);

        tree.Tick(1.0f);

        Assert.Equal(0, root.EvaluateCallCount);
    }

    [Fact]
    public void Tick_ShouldEvaluate_WhenTreeIsEnabled()
    {
        var root = new TestRoot();
        var tree = new TestTree(root);
        tree.Enable();

        tree.Tick(1.0f);

        Assert.Equal(1, root.EvaluateCallCount);
    }

    [Fact]
    public void Reset_ShouldCallResetOnRoot()
    {
        var root = new TestRoot();
        var tree = new TestTree(root);

        tree.Reset();

        Assert.Equal(1, root.ResetCallCount);
    }

    [Fact]
    public void Abort_ShouldCallAbortOnRoot()
    {
        var root = new TestRoot();
        var tree = new TestTree(root);

        tree.Abort();

        Assert.Equal(1, root.AbortCallCount);
    }

    [Fact]
    public void Enable_ShouldSetIsEnabledToTrue()
    {
        var root = new TestRoot();
        var tree = new TestTree(root);

        tree.Enable();

        Assert.True(TreeAccessor.IsEnabled(tree));
    }

    [Fact]
    public void Disable_ShouldSetIsEnabledToFalse()
    {
        var root = new TestRoot();
        var tree = new TestTree(root);

        tree.Disable();

        Assert.False(TreeAccessor.IsEnabled(tree));
    }
}
