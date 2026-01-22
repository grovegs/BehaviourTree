using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree.Tests.Nodes;

public class RootTests
{
    private sealed class TestBlackboard : IBlackboard
    {
        public T GetValue<T>(string key) => default!;
        public void SetValue<T>(string key, T value) where T : notnull { }
        public bool ContainsKey(string key) => false;
        public void DeleteValue(string key) { }
        public void Clear() { }
    }

    private sealed class TestNode : INode
    {
        public int EvaluateCallCount { get; private set; }
        public int ResetCallCount { get; private set; }
        public int AbortCallCount { get; private set; }
        public float LastDeltaTime { get; private set; }
        public NodeState State { get; set; } = NodeState.Success;

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
    }

    [Fact]
    public void Constructor_ShouldInitializeBlackboard()
    {
        var blackboard = new TestBlackboard();

        var root = new BehaviourRoot(blackboard);

        Assert.Equal(blackboard, root.Blackboard);
    }

    [Fact]
    public void Attach_ShouldSetChild_WhenChildIsEmpty()
    {
        var blackboard = new TestBlackboard();
        var root = new BehaviourRoot(blackboard);
        var node = new TestNode();

        root.Attach(node);

        Assert.Throws<ChildAlreadyAttachedException>(() => root.Attach(node));
    }

    [Fact]
    public void Attach_ShouldThrowException_WhenChildAlreadyAttached()
    {
        var blackboard = new TestBlackboard();
        var root = new BehaviourRoot(blackboard);
        var node = new TestNode();
        var secondNode = new TestNode();

        root.Attach(node);

        Assert.Throws<ChildAlreadyAttachedException>(() => root.Attach(secondNode));
    }

    [Fact]
    public void Evaluate_ShouldCallEvaluateOnChild()
    {
        var blackboard = new TestBlackboard();
        var root = new BehaviourRoot(blackboard);
        var node = new TestNode();
        root.Attach(node);

        root.Evaluate(0.5f);

        Assert.Equal(1, node.EvaluateCallCount);
        Assert.Equal(0.5f, node.LastDeltaTime);
    }

    [Fact]
    public void Abort_ShouldCallAbortOnChild()
    {
        var blackboard = new TestBlackboard();
        var root = new BehaviourRoot(blackboard);
        var node = new TestNode();
        root.Attach(node);

        root.Abort();

        Assert.Equal(1, node.AbortCallCount);
    }

    [Fact]
    public void Reset_ShouldCallResetOnChild()
    {
        var blackboard = new TestBlackboard();
        var root = new BehaviourRoot(blackboard);
        var node = new TestNode();
        root.Attach(node);

        root.Reset();

        Assert.Equal(1, node.ResetCallCount);
    }
}
