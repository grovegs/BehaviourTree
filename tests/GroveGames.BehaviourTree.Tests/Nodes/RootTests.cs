using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree.Tests.Nodes;

public class RootTests
{
    private sealed class FakeBlackboard : IBlackboard
    {
        public bool TryGetValue<T>(BlackboardKey<T> key, out T value) { value = default!; return false; }
        public void SetValue<T>(BlackboardKey<T> key, T value) { }
        public void DeleteValue<T>(BlackboardKey<T> key) { }
        public void Clear() { }
    }

    private sealed class FakeNode : INode
    {
        public int EvaluateCallCount { get; private set; }
        public int ResetCallCount { get; private set; }
        public int AbortCallCount { get; private set; }
        public float LastDeltaTime { get; private set; }
        public NodeState State { get; set; } = NodeState.Success;

        public string Name => string.Empty;

        public NodeState Evaluate(float deltaTime)
        {
            EvaluateCallCount++;
            LastDeltaTime = deltaTime;
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

        public void SetParent(IParent parent)
        {
        }

        public void SetName(string name)
        {
        }
    }

    [Fact]
    public void Constructor_ShouldInitializeBlackboard()
    {
        var blackboard = new FakeBlackboard();

        var root = new BehaviourRoot(blackboard);

        Assert.Equal(blackboard, root.Blackboard);
    }

    [Fact]
    public void Attach_ShouldSetChild_WhenChildIsEmpty()
    {
        var blackboard = new FakeBlackboard();
        var root = new BehaviourRoot(blackboard);
        var node = new FakeNode();

        root.Attach(node);

        Assert.Throws<ChildAlreadyAttachedException>(() => root.Attach(node));
    }

    [Fact]
    public void Attach_ShouldThrowException_WhenChildAlreadyAttached()
    {
        var blackboard = new FakeBlackboard();
        var root = new BehaviourRoot(blackboard);
        var node = new FakeNode();
        var secondNode = new FakeNode();

        root.Attach(node);

        Assert.Throws<ChildAlreadyAttachedException>(() => root.Attach(secondNode));
    }

    [Fact]
    public void Evaluate_ShouldCallEvaluateOnChild()
    {
        var blackboard = new FakeBlackboard();
        var root = new BehaviourRoot(blackboard);
        var node = new FakeNode();
        root.Attach(node);

        root.Evaluate(0.5f);

        Assert.Equal(1, node.EvaluateCallCount);
        Assert.Equal(0.5f, node.LastDeltaTime);
    }

    [Fact]
    public void Abort_ShouldCallAbortOnChild()
    {
        var blackboard = new FakeBlackboard();
        var root = new BehaviourRoot(blackboard);
        var node = new FakeNode();
        root.Attach(node);

        root.Abort();

        Assert.Equal(1, node.AbortCallCount);
    }

    [Fact]
    public void Reset_ShouldCallResetOnChild()
    {
        var blackboard = new FakeBlackboard();
        var root = new BehaviourRoot(blackboard);
        var node = new FakeNode();
        root.Attach(node);

        root.Reset();

        Assert.Equal(1, node.ResetCallCount);
    }
}
