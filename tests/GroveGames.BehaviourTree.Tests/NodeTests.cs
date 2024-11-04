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

    public class CooldownTests
    {
        [Fact]
        public void Cooldown_ChildExecutesAfterCooldown()
        {
            // Arrange
            var child = new SuccessNode();
            var cooldown = new Cooldown(child, 1.0f); // 1-second cooldown

            // Act
            var firstResult = cooldown.Evaluate(null, 0); // First evaluation
            Thread.Sleep(1100); // Wait for more than 1 second
            var secondResult = cooldown.Evaluate(null, 0); // Second evaluation

            // Assert
            Assert.Equal(NodeState.SUCCESS, firstResult); // First evaluation should succeed
            Assert.Equal(NodeState.SUCCESS, secondResult); // Second evaluation after cooldown should succeed
        }

        [Fact]
        public void Cooldown_ChildFailsIfCooldownActive()
        {
            // Arrange
            var child = new SuccessNode();
            var cooldown = new Cooldown(child, 1.0f); // 1-second cooldown

            // Act
            var firstResult = cooldown.Evaluate(null, 0); // First evaluation
            var secondResult = cooldown.Evaluate(null, 0); // Immediate second evaluation

            // Assert
            Assert.Equal(NodeState.SUCCESS, firstResult); // First evaluation should succeed
            Assert.Equal(NodeState.FAILURE, secondResult); // Second evaluation should fail due to active cooldown
        }

        [Fact]
        public void Cooldown_CallsChildAfterCooldownExpires()
        {
            // Arrange
            var child = new RunningNode();
            var cooldown = new Cooldown(child, 1.0f); // 1-second cooldown

            // Act
            cooldown.Evaluate(null, 0); // First evaluation to start cooldown
            Thread.Sleep(1100); // Wait for cooldown to expire
            var resultAfterCooldown = cooldown.Evaluate(null, 0);

            // Assert
            Assert.Equal(NodeState.RUNNING, resultAfterCooldown); // Child should run after cooldown
        }
    }

    public class RepeaterTests
    {
        [Fact]
        public void Repeater_FixedCount_RunsChildForSpecifiedTimes()
        {
            // Arrange
            var child = new SuccessNode();
            var repeater = new Repeater(child, RepeatMode.FixedCount, maxCount: 3);

            // Act & Assert
            Assert.Equal(NodeState.RUNNING, repeater.Evaluate(null, 0)); // First repeat
            Assert.Equal(NodeState.RUNNING, repeater.Evaluate(null, 0)); // Second repeat
            Assert.Equal(NodeState.RUNNING, repeater.Evaluate(null, 0)); // Third repeat
            Assert.Equal(NodeState.SUCCESS, repeater.Evaluate(null, 0)); // Final success after max count
        }

        [Fact]
        public void Repeater_UntilSuccess_StopsRepeatingWhenChildSucceeds()
        {
            // Arrange
            var child = new SuccessNode();
            var repeater = new Repeater(child, RepeatMode.UntilSuccess);

            // Act
            var result = repeater.Evaluate(null, 0);

            // Assert
            Assert.Equal(NodeState.SUCCESS, result); // Should stop repeating when child succeeds
        }

        [Fact]
        public void Repeater_UntilFailure_StopsRepeatingWhenChildFails()
        {
            // Arrange
            var child = new FailureNode();
            var repeater = new Repeater(child, RepeatMode.UntilFailure);

            // Act
            var result = repeater.Evaluate(null, 0);

            // Assert
            Assert.Equal(NodeState.FAILURE, result); // Should stop repeating when child fails
        }

        [Fact]
        public void Repeater_Infinite_ReturnsRunningIndefinitely()
        {
            // Arrange
            var child = new SuccessNode();
            var repeater = new Repeater(child, RepeatMode.Infinite);

            // Act
            var result1 = repeater.Evaluate(null, 0);
            var result2 = repeater.Evaluate(null, 0);

            // Assert
            Assert.Equal(NodeState.RUNNING, result1); // Should return RUNNING in Infinite mode
            Assert.Equal(NodeState.RUNNING, result2); // Should continue returning RUNNING
        }

        [Fact]
        public void Repeater_RunningChild_ReturnsRunningUntilComplete()
        {
            // Arrange
            var child = new RunningNode();
            var repeater = new Repeater(child, RepeatMode.FixedCount, maxCount: 2);

            // Act & Assert
            Assert.Equal(NodeState.RUNNING, repeater.Evaluate(null, 0)); // First tick: child is running
            Assert.Equal(NodeState.RUNNING, repeater.Evaluate(null, 0)); // Second tick: child is still running
            // FixedCount should still return RUNNING if child hasn't completed
        }

        [Fact]
        public void Repeater_ResetsCountOnInterrupt()
        {
            // Arrange
            var child = new SuccessNode();
            var repeater = new Repeater(child, RepeatMode.FixedCount, maxCount: 3);

            // Act
            repeater.Evaluate(null, 0); // First evaluation
            repeater.Evaluate(null, 0); // Second evaluation
            repeater.Interrupt(); // Interrupt should reset the count
            var resultAfterInterrupt = repeater.Evaluate(null, 0); // Start again after interrupt

            // Assert
            Assert.Equal(NodeState.RUNNING, resultAfterInterrupt); // Should start from the beginning after interrupt
        }
    }

    public class DecoratorTests
    {
        [Fact]
        public void Decorator_CallsChildMethods()
        {
            // Arrange
            var child = new SuccessNode();
            var decorator = new Decorator(child);

            // Act
            decorator.BeforeEvaluate();
            var result = decorator.Evaluate(null, 0);
            decorator.AfterEvaluate();

            // Assert
            Assert.Equal(NodeState.SUCCESS, result); // Ensures decorator calls child evaluate and returns status
        }

        [Fact]
        public void Decorator_InterruptsChild()
        {
            // Arrange
            var child = new RunningNode();
            var decorator = new Decorator(child);

            // Act
            decorator.Interrupt();

            // No assertion needed for side-effects, but decorator should complete without errors
        }
    }

    public class InverterTests
    {
        [Fact]
        public void Inverter_ReturnsFailureWhenChildSucceeds()
        {
            // Arrange
            var child = new SuccessNode();
            var inverter = new Inverter(child);

            // Act
            var result = inverter.Evaluate(null, 0);

            // Assert
            Assert.Equal(NodeState.FAILURE, result); // Success should invert to Failure
        }

        [Fact]
        public void Inverter_ReturnsSuccessWhenChildFails()
        {
            // Arrange
            var child = new FailureNode();
            var inverter = new Inverter(child);

            // Act
            var result = inverter.Evaluate(null, 0);

            // Assert
            Assert.Equal(NodeState.SUCCESS, result); // Failure should invert to Success
        }

        [Fact]
        public void Inverter_ReturnsRunningWhenChildIsRunning()
        {
            // Arrange
            var child = new RunningNode();
            var inverter = new Inverter(child);

            // Act
            var result = inverter.Evaluate(null, 0);

            // Assert
            Assert.Equal(NodeState.RUNNING, result); // Running should remain Running
        }
    }

    public class SequenceTests
    {

        [Fact]
        public void Sequence_ReturnsRunningIfAnyChildIsRunning()
        {
            // Arrange
            var sequence = new Sequence();
            sequence.AddChild(new SuccessNode())
                    .AddChild(new RunningNode()) // This should cause the sequence to run
                    .AddChild(new SuccessNode());

            NodeState result = NodeState.FAILURE;

            // Act
            for (var i = 0; i < 3; i++)
            {
                result = sequence.Evaluate(null, 0);
            }


            // Assert
            Assert.Equal(NodeState.RUNNING, result);
        }

        [Fact]
        public void Sequence_ReturnsSuccessIfAllChildrenSucceed()
        {
            // Arrange
            var sequence = new Sequence();
            sequence.AddChild(new SuccessNode())
                    .AddChild(new SuccessNode())
                    .AddChild(new SuccessNode());

            NodeState result = NodeState.FAILURE;

            // Act
            for (var i = 0; i < 3; i++)
            {
                result = sequence.Evaluate(null, 0);
            }

            // Assert
            Assert.Equal(NodeState.SUCCESS, result);
        }
    }

    public class ParallelTests
    {
        [Fact]
        public void Parallel_AllSuccessPolicy_ReturnsSuccessIfAllChildrenSucceed()
        {
            // Arrange
            var parallel = new Parallel(ParallelPolicy.ALL_SUCCESS);
            parallel.AddChild(new SuccessNode())
                    .AddChild(new SuccessNode());

            NodeState result = NodeState.FAILURE;

            // Act
            for (var i = 0; i < 2; i++)
            {
                result = parallel.Evaluate(null, 0);
            }

            // Assert
            Assert.Equal(NodeState.SUCCESS, result);
        }

        [Fact]
        public void Parallel_AnySuccessPolicy_ReturnsSuccessIfAnyChildSucceeds()
        {
            // Arrange
            var parallel = new Parallel(ParallelPolicy.ANY_SUCCESS);
            parallel.AddChild(new FailureNode())
                    .AddChild(new SuccessNode())
                    .AddChild(new RunningNode());

            NodeState result = NodeState.FAILURE;

            // Act
            for (var i = 0; i < 3; i++)
            {
                result = parallel.Evaluate(null, 0);
            }

            // Assert
            Assert.Equal(NodeState.SUCCESS, result);
        }

        [Fact]
        public void Parallel_FirstFailurePolicy_ReturnsFailureIfAnyChildFails()
        {
            // Arrange
            var parallel = new Parallel(ParallelPolicy.FIRST_FAILURE);
            parallel.AddChild(new SuccessNode())
                    .AddChild(new FailureNode()) // Should cause parallel to return FAILURE
                    .AddChild(new RunningNode());

            NodeState result = NodeState.FAILURE;

            // Act
            for (var i = 0; i < 3; i++)
            {
                result = parallel.Evaluate(null, 0);
            }

            // Assert
            Assert.Equal(NodeState.FAILURE, result);
        }

        [Fact]
        public void Parallel_ReturnsRunningIfAnyChildIsRunning()
        {
            // Arrange
            var parallel = new Parallel(ParallelPolicy.ALL_SUCCESS);
            parallel.AddChild(new SuccessNode())
                    .AddChild(new RunningNode()) // Should cause parallel to return RUNNING
                    .AddChild(new SuccessNode());

            NodeState result = NodeState.FAILURE;

            // Act
            for (var i = 0; i < 3; i++)
            {
                result = parallel.Evaluate(null, 0);
            }

            // Assert
            Assert.Equal(NodeState.RUNNING, result);
        }
    }

    public class SelectorTests
    {
        [Fact]
        public void Selector_ReturnsRunningIfAnyChildIsRunning()
        {
            // Arrange
            var selector = new Selector();
            selector.AddChild(new FailureNode())
                    .AddChild(new RunningNode()) // Should cause selector to return RUNNING
                    .AddChild(new SuccessNode());

            NodeState result = NodeState.FAILURE;

            // Act
            for (var i = 0; i < 3; i++)
            {
                result = selector.Evaluate(null, 0);
            }

            // Assert
            Assert.Equal(NodeState.RUNNING, result);
        }

        [Fact]
        public void Selector_ReturnsFailureIfAllChildrenFail()
        {
            // Arrange
            var selector = new Selector();
            selector.AddChild(new FailureNode())
                    .AddChild(new FailureNode())
                    .AddChild(new FailureNode()); // All children fail

            NodeState result = NodeState.FAILURE;

            // Act
            for (var i = 0; i < 3; i++)
            {
                result = selector.Evaluate(null, 0);
            }

            // Assert
            Assert.Equal(NodeState.FAILURE, result);
        }
    }
}
