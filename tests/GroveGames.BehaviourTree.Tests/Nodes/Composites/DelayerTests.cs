using System;

using GroveGames.BehaviourTree;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Decorators;

using Moq;

using Xunit;

namespace GroveGames.BehaviourTree.Tests
{
    public class DelayerTests
    {
        [Fact]
        public void Delayer_ReturnsRunning_WhenIntervalIsLessThanWaitTime()
        {
            float waitTime = 2.0f;
            var mockParent = new Mock<IParent>();
            var delayer = new Delayer(mockParent.Object, waitTime);

            // First tick with a deltaTime less than the wait time
            var result = delayer.Evaluate(1.0f);

            Assert.Equal(NodeState.Running, result);
        }

        [Fact]
        public void Delayer_ExecutesChild_WhenIntervalReachesWaitTime()
        {
            float waitTime = 2.0f;
            var mockParent = new Mock<IParent>();
            var mockChild = new Mock<INode>();
            var delayer = new Delayer(mockParent.Object, waitTime);
            delayer.Attach(mockChild.Object); // Attach the child node to the delayer

            // Set up the child node to return SUCCESS when evaluated
            mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

            // First tick (still running as wait time has not been reached)
            Assert.Equal(NodeState.Running, delayer.Evaluate(1.0f));

            // Second tick (reaches wait time)
            var result = delayer.Evaluate(1.0f);

            Assert.Equal(NodeState.Success, result);
            mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Once); // Child should be evaluated once
        }

        [Fact]
        public void Delayer_ResetsInterval_WhenChildCompletes()
        {
            float waitTime = 2.0f;
            var mockParent = new Mock<IParent>();
            var mockChild = new Mock<INode>();
            var delayer = new Delayer(mockParent.Object, waitTime);
            delayer.Attach(mockChild.Object);

            mockChild.Setup(child => child.Evaluate(It.IsAny<float>())).Returns(NodeState.Success);

            // First tick (still running as wait time has not been reached)
            delayer.Evaluate(1.0f);

            // Second tick (reaches wait time, evaluates child, and should reset interval)
            delayer.Evaluate(1.0f);

            // Next tick should be running as the interval has reset
            Assert.Equal(NodeState.Running, delayer.Evaluate(0.5f));
        }

        [Fact]
        public void Delayer_Abort_ResetsInterval()
        {
            float waitTime = 2.0f;
            var mockParent = new Mock<IParent>();
            var mockChild = new Mock<INode>();
            var delayer = new Delayer(mockParent.Object, waitTime);
            delayer.Attach(mockChild.Object);

            // Increment the interval by evaluating once
            delayer.Evaluate(1.5f);

            // Call Abort and check if interval has reset
            delayer.Abort();

            // Since interval is reset, the next evaluation should be running (not executing child)
            Assert.Equal(NodeState.Running, delayer.Evaluate(0.5f));
            mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Never);
        }

        [Fact]
        public void Delayer_Reset_ResetsInterval()
        {
            float waitTime = 2.0f;
            var mockParent = new Mock<IParent>();
            var mockChild = new Mock<INode>();
            var delayer = new Delayer(mockParent.Object, waitTime);
            delayer.Attach(mockChild.Object);

            // Increment the interval by evaluating once
            delayer.Evaluate(1.5f);

            // Call Reset and check if interval has reset
            delayer.Reset();

            // Since interval is reset, the next evaluation should be running (not executing child)
            Assert.Equal(NodeState.Running, delayer.Evaluate(0.5f));
            mockChild.Verify(child => child.Evaluate(It.IsAny<float>()), Times.Never);
        }
    }
}
