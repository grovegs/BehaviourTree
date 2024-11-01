using System.Formats.Tar;

using Godot;

using GroveGames.BehaviourTree;
using GroveGames.BehaviourTree.Nodes.Composites;
using GroveGames.BehaviourTree.Tree;

public partial class Main : Node2D
{
    [Export] PackedScene testScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        testScene.Instantiate();
    }
}