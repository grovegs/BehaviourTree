using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.ActionNode;

namespace GroveGames.BehaviourTree.Tests.Nodes.ActionNodes;
public class ActionNodeTest
{
    [Fact]
    public void Evaluate_ShouldReturnSuccess_WhenActionReturnsSuccess()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockAction = new Mock<Func<NodeState>>();
        mockAction.Setup(a => a()).Returns(NodeState.Success);

        var actionNode = new ActionNode(mockParent.Object, mockAction.Object);

        // Act
        var result = actionNode.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Success, result);
        mockAction.Verify(a => a(), Times.Once); 
    }

    [Fact]
    public void Evaluate_ShouldReturnFailure_WhenActionReturnsFailure()
    {
        // Arrange
        var mockParent = new Mock<IParent>();
        var mockAction = new Mock<Func<NodeState>>();
        mockAction.Setup(a => a()).Returns(NodeState.Failure);

        var actionNode = new ActionNode(mockParent.Object, mockAction.Object);

        // Act
        var result = actionNode.Evaluate(1.0f);

        // Assert
        Assert.Equal(NodeState.Failure, result);
        mockAction.Verify(a => a(), Times.Once);
    }
}
