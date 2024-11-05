
using System.Reflection;

using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;
using GroveGames.BehaviourTree.Nodes.Decorators;

using Parallel = GroveGames.BehaviourTree.Nodes.Composites.Parallel;

namespace GroveGames.BehaviourTree.Tests
{
    // Helper classes for testing
    public class SuccessNode : Node
    {
        public override NodeState Evaluate(IBlackboard blackboard, double delta) => NodeState.SUCCESS;
    }

    public class FailureNode : Node
    {
        public override NodeState Evaluate(IBlackboard blackboard, double delta) => NodeState.FAILURE;
    }

    public class RunningNode : Node
    {
        public override NodeState Evaluate(IBlackboard blackboard, double delta) => NodeState.RUNNING;
    }

    // Mock classes to track calls to Abort and Reset
    public class MockNode : Node
    {
        public bool WasAborted { get; private set; }
        public bool WasReset { get; private set; }

        public override void Abort()
        {
            WasAborted = true;
        }

        public override void Reset()
        {
            WasReset = true;
        }
    }

    public class NodeTests
    {
        [Fact]
        public void Node_SetParent_UpdatesParentReference()
        {
            var parent = new Node();
            var child = new SuccessNode();
            child.SetParent(parent);

            // Use reflection to get the protected `parent` field
            var parentField = typeof(Node).GetField("parent", BindingFlags.NonPublic | BindingFlags.Instance);
            var actualParent = parentField?.GetValue(child);

            Assert.Equal(parent, actualParent);
        }

        [Fact]
        public void Node_Abort_DoesNotThrow()
        {
            var node = new Node();
            node.Abort(); // Should not throw any exception
        }

        [Fact]
        public void Node_Reset_DoesNotThrow()
        {
            var node = new Node();
            node.Reset(); // Should not throw any exception
        }
    }

    public class DecoratorTests
    {
        [Fact]
        public void Decorator_Evaluate_ReturnsChildStatus()
        {
            var child = new SuccessNode();
            var decorator = new Decorator(child);
            var result = decorator.Evaluate(null, 0);
            Assert.Equal(NodeState.SUCCESS, result);
        }

        [Fact]
        public void Decorator_Abort_CallsAbortOnChild()
        {
            var child = new MockNode();
            var decorator = new Decorator(child);
            decorator.Abort();
            Assert.True(child.WasAborted);
        }

        [Fact]
        public void Decorator_Reset_CallsResetOnChild()
        {
            var child = new MockNode();
            var decorator = new Decorator(child);
            decorator.Reset();
            Assert.True(child.WasReset);
        }
    }

    public class SelectorTests
    {
        private static int GetProcessingChildIndex(Node node)
        {
            var field = node.GetType().GetField("processingChildIndex", BindingFlags.NonPublic | BindingFlags.Instance);
            return field != null ? (int)field.GetValue(node) : -1;
        }

        [Fact]
        public void Selector_Evaluate_ReturnsSuccessOnFirstSuccessfulChild()
        {
            var selector = new Selector();
            selector.AddChild(new FailureNode())
                    .AddChild(new SuccessNode());
            _ = selector.Evaluate(null, 0);
            var result = selector.Evaluate(null, 0);
            Assert.Equal(NodeState.SUCCESS, result);
        }

        [Fact]
        public void Selector_Evaluate_ReturnsFailureIfAllChildrenFail()
        {
            var selector = new Selector();
            selector.AddChild(new FailureNode())
                    .AddChild(new FailureNode());
            _ = selector.Evaluate(null, 0);
            _ = selector.Evaluate(null, 0);
            var result = selector.Evaluate(null, 0);
            Assert.Equal(NodeState.FAILURE, result);
        }

        [Fact]
        public void Selector_Abort_ResetsProcessingChildIndex()
        {
            var selector = new Selector();
            selector.AddChild(new RunningNode());
            selector.Evaluate(null, 0); // Set the selector to a running state
            selector.Abort();
            Assert.Equal(0, GetProcessingChildIndex(selector)); // Check if reset via reflection
        }
    }

    public class SequenceTests
    {
        private static int GetProcessingChildIndex(Node node)
        {
            var field = node.GetType().GetField("processingChildIndex", BindingFlags.NonPublic | BindingFlags.Instance);
            return field != null ? (int)field.GetValue(node) : -1;
        }

        [Fact]
        public void Sequence_Evaluate_ReturnsSuccessIfAllChildrenSucceed()
        {
            var sequence = new Sequence();
            sequence.AddChild(new SuccessNode())
                    .AddChild(new SuccessNode());
            _ = sequence.Evaluate(null, 0);
            var result = sequence.Evaluate(null, 0);
            Assert.Equal(NodeState.SUCCESS, result);
        }

        [Fact]
        public void Sequence_Evaluate_ReturnsFailureIfAnyChildFails()
        {
            var sequence = new Sequence();
            sequence.AddChild(new SuccessNode())
                    .AddChild(new FailureNode());
            _ = sequence.Evaluate(null, 0);
            var result = sequence.Evaluate(null, 0);
            Assert.Equal(NodeState.FAILURE, result);
        }

        [Fact]
        public void Sequence_Abort_ResetsProcessingChildIndex()
        {
            var sequence = new Sequence();
            sequence.AddChild(new RunningNode());
            sequence.Evaluate(null, 0); // Set the sequence to a running state
            sequence.Abort();
            Assert.Equal(0, GetProcessingChildIndex(sequence)); // Check if reset via reflection
        }
    }

    public class ParallelTests
    {
        private static int GetProcessingChildIndex(Node node)
        {
            var field = node.GetType().GetField("processingChildIndex", BindingFlags.NonPublic | BindingFlags.Instance);
            return field != null ? (int)field.GetValue(node) : -1;
        }

        [Fact]
        public void Parallel_AllSuccessPolicy_ReturnsSuccessIfAllChildrenSucceed()
        {
            var parallel = new Parallel(ParallelPolicy.ALL_SUCCESS);
            parallel.AddChild(new SuccessNode())
                    .AddChild(new SuccessNode());
            var result = parallel.Evaluate(null, 0);
            Assert.Equal(NodeState.SUCCESS, result);
        }

        [Fact]
        public void Parallel_AnySuccessPolicy_ReturnsSuccessIfAnyChildSucceeds()
        {
            var parallel = new Parallel(ParallelPolicy.ANY_SUCCESS);
            parallel.AddChild(new FailureNode())
                    .AddChild(new SuccessNode());
            var result = parallel.Evaluate(null, 0);
            Assert.Equal(NodeState.SUCCESS, result);
        }

        [Fact]
        public void Parallel_FirstFailurePolicy_ReturnsFailureIfAnyChildFails()
        {
            var parallel = new Parallel(ParallelPolicy.FIRST_FAILURE);
            parallel.AddChild(new SuccessNode())
                    .AddChild(new FailureNode());
            var result = parallel.Evaluate(null, 0);
            Assert.Equal(NodeState.FAILURE, result);
        }

        [Fact]
        public void Parallel_Abort_ResetsProcessingChildIndex()
        {
            var parallel = new Parallel(ParallelPolicy.ANY_SUCCESS);
            parallel.AddChild(new RunningNode());
            parallel.Evaluate(null, 0); // Set the parallel node to a running state
            parallel.Abort();
            Assert.Equal(0, GetProcessingChildIndex(parallel)); // Check if reset via reflection
        }
    }

    public class CooldownTests
    {
        [Fact]
        public void Cooldown_Evaluate_ReturnsFailureIfOnCooldown()
        {
            var child = new SuccessNode();
            var cooldown = new Cooldown(child, 1.0f);
            cooldown.Evaluate(null, 0); // Starts cooldown
            var result = cooldown.Evaluate(null, 0); // Immediately evaluated again
            Assert.Equal(NodeState.FAILURE, result); // Should be on cooldown
        }

        [Fact]
        public void Cooldown_Abort_ResetsCooldownImmediately()
        {
            var child = new SuccessNode();
            var cooldown = new Cooldown(child, 1.0f);
            cooldown.Evaluate(null, 0); // Start cooldown
            cooldown.Abort();
            Assert.Equal(NodeState.SUCCESS, cooldown.Evaluate(null, 0)); // Cooldown should be reset
        }
    }

    public class RepeaterTests
    {
        [Fact]
        public void Repeater_FixedCount_StopsAfterMaxCount()
        {
            var child = new SuccessNode();
            var repeater = new Repeater(child, RepeatMode.FixedCount, maxCount: 3);
            Assert.Equal(NodeState.RUNNING, repeater.Evaluate(null, 0));
            Assert.Equal(NodeState.RUNNING, repeater.Evaluate(null, 0));
            Assert.Equal(NodeState.RUNNING, repeater.Evaluate(null, 0));
            Assert.Equal(NodeState.SUCCESS, repeater.Evaluate(null, 0)); // After max count
        }

        [Fact]
        public void Repeater_Abort_ResetsCurrentCount()
        {
            var child = new SuccessNode();
            var repeater = new Repeater(child, RepeatMode.FixedCount, maxCount: 3);
            repeater.Evaluate(null, 0);
            repeater.Abort();
            Assert.Equal(NodeState.RUNNING, repeater.Evaluate(null, 0)); // Should start from the beginning
        }
    }

    public class InterruptTests
    {
        [Fact]
        public void Interrupt_Evaluate_AbortsChildWhenInterrupted()
        {
            var child = new MockNode();
            var interrupt = new Interrupt(child, () => true);
            interrupt.Evaluate(null, 0);
            Assert.True(child.WasAborted); // Checks if child was aborted
        }
    }
}
