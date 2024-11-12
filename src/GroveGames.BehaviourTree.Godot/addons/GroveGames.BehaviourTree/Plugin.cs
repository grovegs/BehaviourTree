#if TOOLS
using Godot;

[Tool]
public partial class Plugin : EditorPlugin
{
    private Control _window;

    public override void _EnterTree()
    {
        _window = GD.Load<PackedScene>("res://addons/GroveGames.BehaviourTree/BehaviourTreeWindow.tscn").Instantiate<Control>();
        AddControlToDock(DockSlot.LeftBl, _window);
    }

    public override void _ExitTree()
    {
        RemoveControlFromDocks(_window);
    }
}
#endif