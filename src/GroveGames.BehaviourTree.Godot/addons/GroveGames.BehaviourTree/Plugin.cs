#if TOOLS
using Godot;

namespace GroveGames.BehaviourTree;

[Tool]
public partial class Plugin : EditorPlugin
{
    private BehaviourTreeWindow _window;

    public override void _EnterTree()
    {
        _window = new BehaviourTreeWindow();
        AddDebuggerPlugin(_window);
    }

    public override void _ExitTree()
    {
        RemoveDebuggerPlugin(_window);
    }
}
#endif