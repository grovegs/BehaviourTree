using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Godot;

using GroveGames.BehaviourTree;
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
        seq1.Delayer(12f).Abort(() => true);
        seq1.Failer().Succeeder();

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
    private readonly Dictionary<int, Vector2> _nodePositions = [];
    private readonly List<BehviourTreeGraphNode> _nodes = [];

    private Godot.Collections.Dictionary<int, Godot.Collections.Array<int>> _nodeHierarchy;
    private Godot.Collections.Dictionary<int, string> _names;

    private GodotBehaviourTree _tree;

    private int _currentId;

    private const int HorizontalSpacing = 400;
    private const int SubtreeSpacing = 250;
    private const int VerticalSpacing = 200;

    public void Initialize(Godot.Collections.Dictionary<int, Godot.Collections.Array<int>> nodeHierarchy)
    {
        _nodeHierarchy = nodeHierarchy;

        FocusMode = FocusModeEnum.None;

        ClearConnections();
        ClearNodes();
        CalculateNodePositions(_nodeHierarchy.First().Key, 0f, 0f);
        DrawNode(_nodeHierarchy.First().Key);
    }

    public void SetNames(Godot.Collections.Dictionary<int, string> names)
    {
        _names = names;
    }

    public void UpdateStatus(Godot.Collections.Dictionary<int, int> status)
    {
        foreach (var node in _nodes)
        {
            node.SetStatus(status[node.Hash]);
        }
    }


    public void ClearNodes()
    {
        foreach (var node in _nodes)
        {
            node.QueueFree();
        }

        _nodes.Clear();
    }

    private void CalculateNodePositions(int nodeHashCode, float x, float y)
    {
        if (_nodePositions.ContainsKey(nodeHashCode))
            return;

        _nodePositions[nodeHashCode] = new Vector2(x, y);

        var children = GetChildren(nodeHashCode);

        if (children.Count == 0)
            return;

        List<float> subtreeHeights = [];
        var totalSubtreeHeight = 0f;
        foreach (int childHashCode in children)
        {
            float subtreeHeight = (GetSubtreeHeight(childHashCode) - 1) * VerticalSpacing;
            subtreeHeights.Add(subtreeHeight);
            totalSubtreeHeight += subtreeHeight;
        }

        totalSubtreeHeight += (children.Count - 1) * SubtreeSpacing;
        var subtreeYStart = y - totalSubtreeHeight / 2;

        for (int i = 0; i < children.Count; i++)
        {
            float childY = subtreeYStart + subtreeHeights[i] / 2;

            CalculateNodePositions(children[i], x + HorizontalSpacing, childY);

            subtreeYStart += subtreeHeights[i] + SubtreeSpacing;
        }

        var centerY = (_nodePositions[children[0]].Y + _nodePositions[children[^1]].Y) / 2;
        _nodePositions[nodeHashCode] = new Vector2(x, centerY);
    }

    private Godot.Collections.Array<int> GetChildren(int parentHashCode)
    {
        return _nodeHierarchy.ContainsKey(parentHashCode) ? _nodeHierarchy[parentHashCode] : [];
    }

    private int GetSubtreeHeight(int nodeHashCode)
    {
        if (!_nodeHierarchy.ContainsKey(nodeHashCode))
            return 1;

        var children = GetChildren(nodeHashCode);

        if (children.Count == 0)
            return 1;

        var maxHeight = 0;
        foreach (int childHashCode in children)
        {
            maxHeight = Math.Max(maxHeight, GetSubtreeHeight(childHashCode));
        }

        return 1 + maxHeight;
    }

    private void DrawNode(int hashCode)
    {
        if (!_nodePositions.ContainsKey(hashCode))
            return;

        var position = _nodePositions[hashCode];
        var graphNode = new BehviourTreeGraphNode
        {
            PositionOffset = position,
        };
        _nodes.Add(graphNode);
        graphNode.Initialize(_names[hashCode], _currentId, hashCode);
        _currentId++;

        var children = GetChildren(hashCode);

        graphNode.SetSlot(1, true, -1, Colors.White, children.Count > 0, -1, Colors.White);
        AddChild(graphNode);

        foreach (var child in children)
        {
            if (_nodePositions.ContainsKey(child))
            {
                ConnectNode(graphNode.Name, 0, $"{_names[child]}_{_currentId}", 0);
                DrawNode(child);
            }
        }
    }
}
