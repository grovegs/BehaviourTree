using Godot;

using GroveGames.BehaviourTree;
using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;
using GroveGames.BehaviourTree.Nodes.Decorators;

using System;

using Tree = GroveGames.BehaviourTree.Tree;

public partial class TestSceneController : Godot.Node
{
    [Export] Node3D _enemy;
    [Export] Node3D _entity;

    private CharacterBT _characterBT;

    public override void _Ready()
    {
        _characterBT = new CharacterBT();
        _characterBT.SetRoot(new Root(new Blackboard()));
        _characterBT.SetEntity(_entity, _enemy);
        _characterBT.SetupTree();
        _characterBT.Enable();
        AddChild(_characterBT);
    }

    public override void _Process(double delta)
    {
        _characterBT.Tick((float)delta);
    }
}

public partial class CharacterBT : GodotBehaviourTree
{
    private Node3D _entity;
    private Node3D _enemy;


    public void SetEntity(Node3D entity, Node3D enemy)
    {
        _entity = entity;
        _enemy = enemy;
    }

    public override void SetupTree()
    {
        var selector = Root.Selector();

        var attack = selector.Sequence();

        attack
       .Attach(new HasEnemy(_enemy, _entity, attack));
        var attackRepeat = attack.Cooldown(1f).Repeater(RepeatMode.UntilSuccess);
        attackRepeat.Attach(new Attack(attackRepeat));

        var defend = selector.Sequence();
        defend
       .Attach(new HasAttacker(defend));
        var repeat = defend.Cooldown(1f).Repeater(RepeatMode.UntilSuccess);
        repeat.Attach(new Defend(repeat));
    }
}

public class Idle : GroveGames.BehaviourTree.Nodes.Node
{
    public Idle(IParent parent) : base(parent)
    {
    }
}

public class MoveToTarget : GroveGames.BehaviourTree.Nodes.Node
{

    private readonly Node3D _entity;

    private readonly float _speed = 3f;

    public MoveToTarget(IParent parent, Node3D entity) : base(parent)
    {
        _entity = entity;
    }

    public override NodeState Evaluate(float delta)
    {
        var target = Blackboard.GetValue<Node3D>("target_pos_enemey");

        if (target == null)
        {
            return NodeState.Failure;
        }

        var direction = (target.Position - _entity.Position).Normalized();

        Vector3 newPosition = _entity.Position + direction * _speed * (float)delta;
        _entity.Position = newPosition;
        var distance = target.Position.DistanceTo(_entity.Position);

        GD.Print("Moving");

        if (distance <= 1f)
        {
            Blackboard.DeleteValue("target_pos_enemey");
            return NodeState.Success;
        }

        return NodeState.Running;
    }
}

public class Attack : GroveGames.BehaviourTree.Nodes.Node
{
    public Attack(IParent parent) : base(parent)
    {
    }

    public override NodeState Evaluate(float delta)
    {
        return _nodeState = NodeState.Success;
    }
}

public class Input : GroveGames.BehaviourTree.Nodes.Node
{
    private readonly Node3D _entity;

    public Input(Node3D entity, IParent parent) : base(parent)
    {
        _entity = entity;
    }

    public override NodeState Evaluate(float delta)
    {
        Vector3 direction = Vector3.Zero;

        if (Godot.Input.IsActionPressed("move_forward"))
        { direction.Z -= 1; }
        if (Godot.Input.IsActionPressed("move_backward"))
            direction.Z += 1;
        if (Godot.Input.IsActionPressed("move_left"))
            direction.X -= 1;
        if (Godot.Input.IsActionPressed("move_right"))
            direction.X += 1;

        direction = direction.Normalized();
        if (direction == Vector3.Zero)
        {
            return NodeState.Failure;
        }
        _entity.Position += direction * 5f * (float)delta;
        return NodeState.Running;
    }
}

public class Defend : GroveGames.BehaviourTree.Nodes.Node
{
    public Defend(IParent parent) : base(parent)
    {
    }

    public override NodeState Evaluate(float delta)
    {
        return _nodeState = NodeState.Success;
    }
}

public class HasAttacker : GroveGames.BehaviourTree.Nodes.Node
{
    public HasAttacker(IParent parent) : base(parent)
    {
    }

    public override NodeState Evaluate(float delta)
    {
        return _nodeState = NodeState.Success;
    }
}

public class HasEnemy : GroveGames.BehaviourTree.Nodes.Node
{
    private readonly Node3D _enemy;
    private readonly Node3D _entity;

    public HasEnemy(Node3D enemy, Node3D entity, IParent parent) : base(parent)
    {
        _enemy = enemy;
        _entity = entity;
    }

    public override NodeState Evaluate(float delta)
    {
        return _nodeState = NodeState.Success;
    }
}
