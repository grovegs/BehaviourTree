using Godot;

using GroveGames.BehaviourTree.Nodes;

namespace GroveGames.BehaviourTree;

[Tool]
public sealed partial class BehviourTreeGraphNode : GraphNode
{
    private StyleBoxFlat _styleBoxFlat;

    private int _id;
    private bool _intialized;
    private int _hashCode;
    public int Hash => _hashCode;

    public void Initialize(string name, int id, int hashCode)
    {
        _id = id;
        _hashCode = hashCode;
        Draggable = false;
        Name = $"{name}_{_id}";
        Title = name;

        _styleBoxFlat = new StyleBoxFlat();

        var left = new Control();
        var right = new Control();

        AddChild(left);
        AddChild(right);

        _intialized = true;
    }

    public void SetStatus(int status)
    {
        SetColor((NodeState)status);
    }

    private void SetColor(NodeState state)
    {
        var color = state switch
        {
            NodeState.Success => new Color(0, 1, 0),
            NodeState.Running => new Color(1, 1, 0),
            NodeState.Failure => new Color(1, 0, 0),
            _ => new Color(0.5f, 0.5f, 0.5f),
        };
        _styleBoxFlat.BgColor = color;
        AddThemeStyleboxOverride("panel", _styleBoxFlat);
    }
}
