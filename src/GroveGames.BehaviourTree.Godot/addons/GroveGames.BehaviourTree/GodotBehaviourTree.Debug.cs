#if TOOLS
using System.Collections.Generic;
using System.Reflection;

using Godot;

using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;
using GroveGames.BehaviourTree.Nodes.Decorators;

namespace GroveGames.BehaviourTree;

public abstract partial class GodotBehaviourTree
{
    private int _counter;

    private bool _debuggable;

    [Export]
    private bool Debuggable
    {
        get
        {
            return _debuggable;
        }

        set
        {
            if (value)
            {
                FillDebugData();
            }
            _debuggable = value;
        }
    }

    private Godot.Collections.Dictionary<int, int> _statusData;

    public override void _Process(double delta)
    {
        if (!Debuggable)
        {
            return;
        }

        _counter++;

        if (_counter % 25 == 0)
        {
            _counter = 1;
            if (_root != null && _statusData != null)
            {
                FillStatusData(_root);
                EngineDebugger.SendMessage("BehaviourTree:Status", [_statusData]);
            }
        }
    }

    private void FillDebugData()
    {
        var nodeData = new Godot.Collections.Dictionary<int, Godot.Collections.Array<int>>();
        var nameData = new Godot.Collections.Dictionary<int, string>();
        _statusData = [];
        FillNodeData(nodeData, _root);
        FillNameData(nameData, _root);
        FillStatusData(_root);
        EngineDebugger.SendMessage("BehaviourTree:Tree", [GetType().Name]);
        EngineDebugger.SendMessage("BehaviourTree:Names", [nameData]);
        EngineDebugger.SendMessage("BehaviourTree:Nodes", [nodeData]);
        EngineDebugger.SendMessage("BehaviourTree:Status", [_statusData]);
    }

    private void FillStatusData(INode node)
    {
        var hashCode = node.GetHashCode();

        if (_statusData.ContainsKey(hashCode))
        {
            _statusData[hashCode] = (int)node.State;
        }
        else
        {
            _statusData.Add(hashCode, (int)node.State);
        }

        var childs = GetChildren(node);

        foreach (var child in childs)
        {
            FillStatusData(child);
        }
    }

    private void FillNameData(Godot.Collections.Dictionary<int, string> data, INode node)
    {
        data.Add(node.GetHashCode(), node.GetType().Name);

        var childs = GetChildren(node);

        foreach (var child in childs)
        {
            FillNameData(data, child);
        }
    }

    private void FillNodeData(Godot.Collections.Dictionary<int, Godot.Collections.Array<int>> data, INode root)
    {
        var childs = GetChildren(root);

        if (childs.Count == 0)
        {
            data.Add(root.GetHashCode(), []);
            return;
        }

        var childArray = new Godot.Collections.Array<int>();

        foreach (var child in childs)
        {
            childArray.Add(child.GetHashCode());
        }

        data.Add(root.GetHashCode(), childArray);

        foreach (var child in childs)
        {
            FillNodeData(data, child);
        }

    }

    private List<INode> GetChildren(INode node)
    {
        List<INode> children = [];

        if (node is Composite composite)
        {
            var childrenField = typeof(Composite).GetField("_children", BindingFlags.NonPublic | BindingFlags.Instance);
            if (childrenField != null)
            {
                if (childrenField.GetValue(composite) is List<INode> childNodes)
                {
                    children.AddRange(childNodes);
                }
            }
        }
        else if (node is Decorator decorator)
        {
            var childField = typeof(Decorator).GetField("_child", BindingFlags.NonPublic | BindingFlags.Instance);
            if (childField != null)
            {
                if (childField.GetValue(decorator) is INode singleChild && singleChild != Nodes.Node.Empty)
                {
                    children.Add(singleChild);
                }
            }
        }
        else if (node is Root root)
        {
            var childField = typeof(Root).GetField("_child", BindingFlags.NonPublic | BindingFlags.Instance);
            if (childField != null)
            {
                if (childField.GetValue(root) is INode rootChild && rootChild != Nodes.Node.Empty)
                {
                    children.Add(rootChild);
                }
            }
        }

        return children;
    }
}
#endif
