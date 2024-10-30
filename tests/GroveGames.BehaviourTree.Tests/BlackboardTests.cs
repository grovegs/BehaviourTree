using System;

using GroveGames.BehaviourTree;
using GroveGames.BehaviourTree.Nodes;

using Xunit;

namespace GroveGames.BehaviourTree.Tests
{
    public class BlackboardTests
    {
        [Fact]
        public void SetValue_StoresValueInBlackboard()
        {
            var blackboard = new Blackboard();
            blackboard.SetValue("test_key", 42);

            var value = blackboard.GetValue<int>("test_key");

            Assert.Equal(42, value);
        }

        [Fact]
        public void GetValue_ReturnsDefaultValueIfKeyNotFound()
        {
            var blackboard = new Blackboard();

            var value = blackboard.GetValue("missing_key", -1);

            Assert.Equal(-1, value);
        }

        [Fact]
        public void DeleteValue_SetsValueToNull()
        {
            var blackboard = new Blackboard();
            blackboard.SetValue("test_key", 42);

            blackboard.DeleteValue("test_key");
            var value = blackboard.GetValue<int>("test_key", 0);

            Assert.Equal(0, value);
        }
    }

    public class NodeTests
    {
        [Fact]
        public void Node_Evaluate_ReturnsFailure()
        {
            var blackboard = new Blackboard();
            var node = new Node(blackboard);

            Assert.Equal(NodeState.FAILURE, node.Evaluate());
        }
    }
}
