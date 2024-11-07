using Godot;

using GroveGames.BehaviourTree;
using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes;
using GroveGames.BehaviourTree.Nodes.Composites;

using System;

using Tree = GroveGames.BehaviourTree.Tree;

public partial class TestSceneController : Godot.Node
{
    [Export] Node3D _enemy;
    [Export] Node3D _entity;

    private CharacterBT _characterBT;
    private readonly IBlackboard _blackboard = new Blackboard();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _characterBT = new CharacterBT(new Root(new Blackboard()));
        _characterBT.SetEntity(_entity, _enemy);
        _characterBT.SetupTree();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        _characterBT.Tick((float)delta);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsPressed())
        {
            _characterBT.Abort();
        }
    }
}

public class CharacterBT : Tree
{
    private Node3D _entity;
    private Node3D _enemy;

    public CharacterBT(GroveGames.BehaviourTree.Nodes.Root root) : base(root)
    {
    }

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
       .Attach(new LookForEnemy(_enemy, _entity, attack))
       .Attach(new MoveToTarget(attack, _entity))
       .Attach(new Attack(selector));

        var gather = selector.Sequence();

        gather
       .Attach(new LookForGathering(gather))
       .Attach(new MoveToTarget(gather, _entity))
       .Attach(new Gather(gather));
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
        return NodeState.Failure;
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

public class Gather : GroveGames.BehaviourTree.Nodes.Node
{
    public Gather(IParent parent) : base(parent)
    {
    }

    public override NodeState Evaluate(float delta)
    {
        return NodeState.Failure;
    }
}

public class LookForGathering : GroveGames.BehaviourTree.Nodes.Node
{
    public LookForGathering(IParent parent) : base(parent)
    {
    }

    public override NodeState Evaluate(float delta)
    {
        return NodeState.Failure;
    }
}

public class LookForEnemy : GroveGames.BehaviourTree.Nodes.Node
{
    private readonly Node3D _enemy;
    private readonly Node3D _entity;

    public LookForEnemy(Node3D enemy, Node3D entity, IParent parent) : base(parent)
    {
        _enemy = enemy;
        _entity = entity;
    }

    public override NodeState Evaluate(float delta)
    {
        if (_enemy == null)
        {
            return NodeState.Failure;
        }

        var distance = _enemy.Position.DistanceTo(_entity.Position);

        if (distance <= 10f)
        {
            Blackboard.SetValue("target_pos_enemey", _enemy);
            return NodeState.Success;
        }

        return NodeState.Failure;

    }
}
