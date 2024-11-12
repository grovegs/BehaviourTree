using System;
using System.Collections.Generic;
using System.Reflection;

using Godot;

using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;
using GroveGames.BehaviourTree.Nodes.Decorators;


public class TestTree : GroveGames.BehaviourTree.Tree
{
    public TestTree(IRoot root) : base(root)
    {
    }

    public override void SetupTree()
    {
        var selector = Root.Selector();
        var seq1 = selector.Sequence();
        seq1.Cooldown(1f).Abort(() => true);
        seq1.Delayer(1f);

        var seq2 = selector.Sequence();
        seq2.Repeater(RepeatMode.UntilSuccess);
        seq2.Failer();

        var seq3 = selector.Sequence();
        seq3.Failer();
        seq3.Succeeder();


        var seq4 = selector.Sequence();
        seq4.Failer();
        seq4.Succeeder();

        var seq5 = selector.Sequence();
        seq5.Failer();
        seq5.Succeeder();
    }
}

[Tool]
public partial class BehaviourTreeGraph : GraphEdit
{
    private readonly Dictionary<INode, Vector2> _nodePositions = [];
    private Root _rootNode;

    private int _currentId;

    private const int HorizontalSpacing = 400;
    private const int SubtreeSpacing = 250;
    private const int VerticalSpacing = 200;

    public override void _EnterTree()
    {
        FocusMode = FocusModeEnum.None;

        var test = new TestTree(new Root(new Blackboard()));
        test.SetupTree();

        _rootNode = typeof(GroveGames.BehaviourTree.Tree).GetField("_root", BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(test) as Root;

        CalculateNodePositions(_rootNode, 0f, 0f);
        DrawNode(_rootNode);
    }

    private void CalculateNodePositions(INode node, float x, float y)
    {
        if (node == null || _nodePositions.ContainsKey(node))
            return;

        var children = GetChildren(node);

        if (children.Count == 0)
        {
            _nodePositions[node] = new Vector2(x, y);
            return;
        }

        List<float> subtreeHeights = [];
        float totalSubtreeHeight = 0;

        foreach (var child in children)
        {
            float subtreeHeight = (GetSubtreeHeight(child) - 1) * VerticalSpacing;
            subtreeHeights.Add(subtreeHeight);
            totalSubtreeHeight += subtreeHeight;
        }

        totalSubtreeHeight += (children.Count - 1) * SubtreeSpacing;
        var subtreeYStart = y - totalSubtreeHeight / 2;

        for (int i = 0; i < children.Count; i++)
        {
            var childY = subtreeYStart + subtreeHeights[i] / 2;
            CalculateNodePositions(children[i], x + HorizontalSpacing, childY);
            subtreeYStart += subtreeHeights[i] + SubtreeSpacing;
        }


        var centerY = (_nodePositions[children[0]].Y + _nodePositions[children[^1]].Y) / 2;
        _nodePositions[node] = new Vector2(x, centerY);
    }

    private int GetSubtreeHeight(INode node)
    {
        if (node == null)
            return 0;

        var children = GetChildren(node);

        if (children.Count == 0)
            return 1;

        var maxHeight = 0;
        foreach (var child in children)
        {
            maxHeight = Math.Max(maxHeight, GetSubtreeHeight(child));
        }

        return 1 + maxHeight;
    }

    private List<INode> GetChildren(INode node)
    {
        List<INode> children = new List<INode>();

        if (node is Composite comp)
        {
            var childrenField = typeof(Composite).GetField("_children", BindingFlags.NonPublic | BindingFlags.Instance);
            if (childrenField != null)
            {
                var childNodes = childrenField.GetValue(comp) as List<INode>;
                if (childNodes != null)
                {
                    children.AddRange(childNodes);
                }
            }
        }
        else if (node is Decorator dec)
        {
            var childField = typeof(Decorator).GetField("_child", BindingFlags.NonPublic | BindingFlags.Instance);
            if (childField != null)
            {
                var singleChild = childField.GetValue(dec) as INode;
                if (singleChild != null && singleChild != GroveGames.BehaviourTree.Nodes.Node.Empty)
                {
                    children.Add(singleChild);
                }
            }
        }
        else if (node is Root root)
        {
            FieldInfo childField = root.GetType().GetField("_child", BindingFlags.NonPublic | BindingFlags.Instance);
            if (childField != null)
            {
                var rootChild = childField.GetValue(root) as INode;
                if (rootChild != null && rootChild != GroveGames.BehaviourTree.Nodes.Node.Empty)
                {
                    children.Add(rootChild);
                }
            }
        }

        return children;
    }

    private void DrawNode(INode node)
    {
        if (node == null || !_nodePositions.ContainsKey(node))
            return;

        var position = _nodePositions[node];

        var graphNode = new GraphNode
        {
            PositionOffset = position,
            Title = node.GetType().Name,
            Name = node.GetType().Name + "_" + _currentId
        };
        graphNode.Draggable = false;
        graphNode.AddChild(new Control());
        graphNode.AddChild(new Control());
        _currentId++;

        var children = GetChildren(node);

        graphNode.SetSlot(1, true, -1, Colors.Yellow, children.Count > 0, -1, Colors.Blue);

        AddChild(graphNode);

        foreach (var child in children)
        {
            if (_nodePositions.ContainsKey(child))
            {
                ConnectNode(graphNode.Name, 0, child.GetType().Name + "_" + _currentId, 0);
                DrawNode(child);
            }
        }
    }

}
