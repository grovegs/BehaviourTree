using System;
using System.Reflection;

using GroveGames.BehaviourTree;
using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;

using Xunit;

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

    public class SelectorTests
    {
        private int GetProcessingChild(Selector selector)
        {
            // Use reflection to access the private processingChild field
            var field = typeof(Selector).GetField("processingChild", BindingFlags.NonPublic | BindingFlags.Instance);
            return (int)field.GetValue(selector);
        }

        [Fact]
        public void Selector_StopsOnFirstSuccess_ReturnsSuccess()
        {
            // Arrange
            var selector = new Selector();
            selector.AddChild(new FailureNode())
                    .AddChild(new SuccessNode())
                    .AddChild(new RunningNode());

            // Act
            selector.Evaluate(null, 0);
            var result = selector.Evaluate(null, 0);

            // Assert
            Assert.Equal(NodeState.SUCCESS, result);
            Assert.Equal(0, GetProcessingChild(selector)); // processingChild should reset after success
        }

        [Fact]
        public void Selector_ReturnsRunningIfAnyChildIsRunning()
        {
            // Arrange
            var selector = new Selector();
            selector.AddChild(new FailureNode())
                    .AddChild(new RunningNode())
                    .AddChild(new SuccessNode());

            // Act
            selector.Evaluate(null, 0);
            selector.Evaluate(null, 0);
            selector.Evaluate(null, 0);
            var result = selector.Evaluate(null, 0); ;

            // Assert
            Assert.Equal(NodeState.RUNNING, result);
            Assert.Equal(1, GetProcessingChild(selector)); // processingChild should point to the running node
        }

        [Fact]
        public void Selector_ContinuesAfterFailure_ReturnsFailureIfAllFail()
        {
            // Arrange
            var selector = new Selector();
            selector.AddChild(new FailureNode())
                    .AddChild(new FailureNode())
                    .AddChild(new FailureNode());

            // Act
            selector.Evaluate(null, 0);
            selector.Evaluate(null, 0);
            var result = selector.Evaluate(null, 0);

            // Assert
            Assert.Equal(NodeState.FAILURE, result);
            Assert.Equal(0, GetProcessingChild(selector)); // Should reset after all children fail
        }

        [Fact]
        public void Selector_MovesToNextChildAfterFailure()
        {
            // Arrange
            var selector = new Selector();
            selector.AddChild(new FailureNode())
                    .AddChild(new RunningNode())
                    .AddChild(new SuccessNode());

            // Act - Tick 1: First child fails, move to next
            var result1 = selector.Evaluate(null, 0);
            Assert.Equal(NodeState.FAILURE, result1);
            Assert.Equal(1, GetProcessingChild(selector)); // Should be pointing at RunningNode

            // Act - Tick 2: Running node is still running
            var result2 = selector.Evaluate(null, 0);
            Assert.Equal(NodeState.RUNNING, result2);
            Assert.Equal(1, GetProcessingChild(selector)); // Should stay at RunningNode
        }

        [Fact]
        public void Selector_ResetsProcessingChildOnSuccess()
        {
            // Arrange
            var selector = new Selector();
            selector.AddChild(new FailureNode())
                    .AddChild(new SuccessNode());

            // Act
            selector.Evaluate(null, 0);
            var result = selector.Evaluate(null, 0);

            // Assert
            Assert.Equal(NodeState.SUCCESS, result);
            Assert.Equal(0, GetProcessingChild(selector)); // processingChild should reset after success
        }

        [Fact]
        public void Selector_Interrupt_ShouldResetProcessingChild()
        {
            // Arrange
            var selector = new Selector();
            selector.AddChild(new FailureNode())
                    .AddChild(new RunningNode());

            // Act
            selector.Evaluate(null, 0); // Fail first child
            selector.Evaluate(null, 0); // Start running second child
            selector.Interrupt();

            // Assert
            Assert.Equal(0, GetProcessingChild(selector)); // processingChild should reset after interrupt
        }
    }
}
