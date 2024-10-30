using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;

using Parallel = GroveGames.BehaviourTree.Nodes.Composites.Parallel;

namespace GroveGames.BehaviourTree.Tests
{
    public class CompositeNodeTests
    {
        private class SuccessNode : Node
        {
            public SuccessNode(Blackboard blackboard) : base(blackboard) { }

            public override NodeState Evaluate() => NodeState.SUCCESS;
        }

        private class RunningNode : Node
        {
            public RunningNode(Blackboard blackboard) : base(blackboard) { }

            public override NodeState Evaluate() => NodeState.RUNNING;
        }

        private class FailureNode : Node
        {
            public FailureNode(Blackboard blackboard) : base(blackboard) { }

            public override NodeState Evaluate() => NodeState.FAILURE;
        }

        [Fact]
        public void Sequence_Evaluate_ReturnsSuccessWhenAllChildrenSucceed()
        {
            var blackboard = new Blackboard();
            var sequence = new Sequence(blackboard);
            sequence.AddChild(new SuccessNode(blackboard))
                    .AddChild(new SuccessNode(blackboard));

            Assert.Equal(NodeState.SUCCESS, sequence.Evaluate());
        }

        [Fact]
        public void Sequence_Evaluate_ReturnsFailureWhenAnyChildFails()
        {
            var blackboard = new Blackboard();
            var sequence = new Sequence(blackboard);
            sequence.AddChild(new SuccessNode(blackboard))
                    .AddChild(new FailureNode(blackboard));

            Assert.Equal(NodeState.FAILURE, sequence.Evaluate());
        }

        [Fact]
        public void Sequence_Evaluate_ReturnsRunningWhenChildIsRunning()
        {
            var blackboard = new Blackboard();
            var sequence = new Sequence(blackboard);
            sequence.AddChild(new SuccessNode(blackboard))
                    .AddChild(new RunningNode(blackboard));

            Assert.Equal(NodeState.RUNNING, sequence.Evaluate());
        }

        [Fact]
        public void Selector_Evaluate_ReturnsSuccessWhenAnyChildSucceeds()
        {
            var blackboard = new Blackboard();
            var selector = new Selector(blackboard);
            selector.AddChild(new FailureNode(blackboard))
                    .AddChild(new SuccessNode(blackboard));

            Assert.Equal(NodeState.SUCCESS, selector.Evaluate());
        }

        [Fact]
        public void Selector_Evaluate_ReturnsFailureWhenAllChildrenFail()
        {
            var blackboard = new Blackboard();
            var selector = new Selector(blackboard);
            selector.AddChild(new FailureNode(blackboard))
                    .AddChild(new FailureNode(blackboard));

            Assert.Equal(NodeState.FAILURE, selector.Evaluate());
        }

        [Fact]
        public void Selector_Evaluate_ReturnsRunningWhenChildIsRunning()
        {
            var blackboard = new Blackboard();
            var selector = new Selector(blackboard);
            selector.AddChild(new FailureNode(blackboard))
                    .AddChild(new RunningNode(blackboard));

            Assert.Equal(NodeState.RUNNING, selector.Evaluate());
        }

        [Fact]
        public void Parallel_Evaluate_ReturnsSuccessWhenPolicyIsAllSuccessAndAllChildrenSucceed()
        {
            var blackboard = new Blackboard();
            var parallel = new Parallel(blackboard, ParallelPolicy.ALL_SUCCESS);
            parallel.AddChild(new SuccessNode(blackboard))
                    .AddChild(new SuccessNode(blackboard));

            Assert.Equal(NodeState.SUCCESS, parallel.Evaluate());
        }

        [Fact]
        public void Parallel_Evaluate_ReturnsFailureWhenPolicyIsAllSuccessAndAnyChildFails()
        {
            var blackboard = new Blackboard();
            var parallel = new Parallel(blackboard, ParallelPolicy.ALL_SUCCESS);
            parallel.AddChild(new SuccessNode(blackboard))
                    .AddChild(new FailureNode(blackboard));

            Assert.Equal(NodeState.FAILURE, parallel.Evaluate());
        }

        [Fact]
        public void Parallel_Evaluate_ReturnsRunningWhenPolicyIsAnySuccessAndChildIsRunning()
        {
            var blackboard = new Blackboard();
            var parallel = new Parallel(blackboard, ParallelPolicy.ANY_SUCCESS);
            parallel.AddChild(new RunningNode(blackboard))
                    .AddChild(new FailureNode(blackboard));

            Assert.Equal(NodeState.RUNNING, parallel.Evaluate());
        }

        [Fact]
        public void Parallel_Evaluate_ReturnsSuccessWhenPolicyIsAnySuccessAndAnyChildSucceeds()
        {
            var blackboard = new Blackboard();
            var parallel = new Parallel(blackboard, ParallelPolicy.ANY_SUCCESS);
            parallel.AddChild(new FailureNode(blackboard))
                    .AddChild(new SuccessNode(blackboard));

            Assert.Equal(NodeState.SUCCESS, parallel.Evaluate());
        }

        [Fact]
        public void Parallel_Evaluate_ReturnsFailureWhenPolicyIsFirstFailureAndAnyChildFails()
        {
            var blackboard = new Blackboard();
            var parallel = new Parallel(blackboard, ParallelPolicy.FIRST_FAILURE);
            parallel.AddChild(new SuccessNode(blackboard))
                    .AddChild(new FailureNode(blackboard));

            Assert.Equal(NodeState.FAILURE, parallel.Evaluate());
        }
    }
}
