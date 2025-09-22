using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.ActionNodes;

namespace GroveGames.BehaviourTree.Tests.Nodes.ActionNodes;
public class ConditionNodeTest
{
    [Fact]
    public static void Evaluate_ShouldReturnSuccess_WhenConditionIsTrue()
    { 
        // Arrange
        var mockParent = new Mock<IParent>();
        var condition = new Mock<Func<bool>>();
        condition.Setup(c => c()).Returns(true);

        var conditionNode = new ConditionNode(mockParent.Object, condition.Object);
        // Act
        var result = conditionNode.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, result);
    }
    [Fact]
    public static void Evaluate_ShouldReturnFailure_WhenConditionIsFalse()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var condition = new Mock<Func<bool>>();
        condition.Setup(c => c()).Returns(false);
        var conditionNode = new ConditionNode(mockParent.Object, condition.Object);

        // Act
        var result = conditionNode.Evaluate(1.0f);
        
        // Assert
        Assert.Equal(NodeState.Failure, result);
    }
}
