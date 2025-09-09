#if TOOLS
using Godot;

namespace GroveGames.BehaviourTree;

[Tool]
public sealed partial class BehaviourTreeWindow : EditorDebuggerPlugin
{
    private BehaviourTreeGraph _behaviourTreeGraph = new();
    private Label _currentTreeLabel = new();

    public override bool _HasCapture(string capture)
    {
        return capture == "BehaviourTree";
    }

    public override bool _Capture(string message, Godot.Collections.Array data, int sessionId)
    {
        if (_behaviourTreeGraph == null)
        {
            return false;
        }

        if (message == "BehaviourTree:Tree")
        {
            var treeName = data[0].As<string>();
            _currentTreeLabel.Text = $"Current Tree: {treeName}";
            return true;
        }

        if (message == "BehaviourTree:Nodes")
        {
            var nodes = data[0].AsGodotDictionary<int, Godot.Collections.Array<int>>();
            _behaviourTreeGraph.Initialize(nodes);
            return true;
        }

        if (message == "BehaviourTree:Names")
        {
            var names = data[0].AsGodotDictionary<int, string>();
            _behaviourTreeGraph.SetNames(names);
            return true;
        }

        if (message == "BehaviourTree:Status")
        {
            var status = data[0].AsGodotDictionary<int, int>();
            _behaviourTreeGraph.UpdateStatus(status);
            return true;
        }

        return false;
    }

    public override void _SetupSession(int sessionId)
    {
        var panel = new Control
        {
            Name = "Behaviour Tree Debugger"
        };
        panel.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _behaviourTreeGraph.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        panel.AddChild(_behaviourTreeGraph);

        _currentTreeLabel = new Label
        {
            Size = new Vector2(100f, 200f),
            Text = "Current Tree: None",
            Name = "CurrentTreeLabel"
        };
        _currentTreeLabel.SetAnchorsPreset(Control.LayoutPreset.TopLeft);
        _currentTreeLabel.OffsetTop = 75;
        panel.AddChild(_currentTreeLabel);

        var button = new Button()
        {
            Size = new Vector2(150f, 50f),
            Text = "Clear",
        };

        button.Pressed += () =>
        {
            _behaviourTreeGraph.ClearGraph();
            _currentTreeLabel.Text = "Current Tree: None";
        };

        button.SetAnchorsPreset(Control.LayoutPreset.TopLeft);
        button.OffsetTop = 150;
        panel.AddChild(button);

        var session = GetSession(sessionId);
        session.AddSessionTab(panel);
    }

    private Godot.Node FindByInstanceIdRecursive(Godot.Node node, ulong instanceId)
    {
        if (node.GetInstanceId() == instanceId)
        {
            return node;
        }

        var childs = node.GetChildren();

        foreach (var child in childs)
        {
            if (child.GetInstanceId() == instanceId)
            {
                return child;
            }

            return FindByInstanceIdRecursive(child, instanceId);
        }

        return null;
    }
}
#endif
