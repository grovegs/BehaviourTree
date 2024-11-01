using Godot;

using GroveGames.BehaviourTree;
using GroveGames.BehaviourTree.Collections;
using GroveGames.BehaviourTree.Nodes.Composites;
using GroveGames.BehaviourTree.Tree;

using System;

public partial class TestSceneController : Node
{
	[Export] Node3D _enemy;
	[Export] Node3D _entity;

	private CharacterBT _characterBT;
    private readonly IBlackboard _blackboard = new Blackboard();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_characterBT = new CharacterBT();
		_characterBT.SetEntity(_entity, _enemy);
		_characterBT.SetupTree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_characterBT.Tick(_blackboard, delta);
	}

    public override void _Input(InputEvent @event)
    {
        if (@event.IsPressed())
        {
            _characterBT.Interrupt();
        }
    }
}

public class CharacterBT : BehaviourTree
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
       var selector = new Selector();

       var inputSequence = new Input(_entity);
       var attackSequence = new Sequence()
       .AddChild(new LookForEnemy(_enemy, _entity))
       .AddChild(new MoveToTarget(_entity))
       .AddChild(new Attack());

       var gatherSequnce = new Sequence()
       .AddChild(new LookForGathering())
       .AddChild(new MoveToTarget(_entity))
       .AddChild(new Gather());

       selector
       .AddChild(inputSequence)
       .AddChild(attackSequence)
       .AddChild(gatherSequnce);

       root = selector;
    }

    public override void Tick(IBlackboard blackboard, double delta)
    {
        root.BeforeEvaluate();
        root.Evaluate(blackboard, delta);
        root.AfterEvaluate();
    }
}

public class Idle : GroveGames.BehaviourTree.Nodes.Node
{

}

public class MoveToTarget : GroveGames.BehaviourTree.Nodes.Node
{
    
    private readonly Node3D _entity;

    private readonly float _speed = 3f;

    public MoveToTarget(Node3D entity) : base()
    {
        _entity = entity;
    }

    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
		var target = blackboard.GetValue<Node3D>("target_pos_enemey");

		if (target == null)
		{
			return NodeState.FAILURE;
		}

        var direction = (target.Position - _entity.Position).Normalized();

        Vector3 newPosition = _entity.Position + direction * _speed  * (float)delta;
		_entity.Position = newPosition;
        var distance = target.Position.DistanceTo(_entity.Position);

        GD.Print("Moving");

        if (distance <= 1f)
        {
            blackboard.DeleteValue("target_pos_enemey");
            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }
}

public class Attack : GroveGames.BehaviourTree.Nodes.Node
{
    public Attack() : base()
    {
    }

    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        GD.Print("Attacking");

        return NodeState.FAILURE;
    }
}

public class Input : GroveGames.BehaviourTree.Nodes.Node
{
    private readonly Node3D _entity;

    public Input(Node3D entity) : base()
    {
        _entity = entity;
    }

    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
    
          Vector3 direction = Vector3.Zero;

        if (Godot.Input.IsActionPressed("move_forward"))
            {direction.Z -= 1;}
        if (Godot.Input.IsActionPressed("move_backward"))
            direction.Z += 1;
        if (Godot.Input.IsActionPressed("move_left"))
            direction.X -= 1;
        if (Godot.Input.IsActionPressed("move_right"))
            direction.X += 1;

        direction = direction.Normalized();
        if (direction == Vector3.Zero)
        {
            return NodeState.FAILURE;
        }
        _entity.Position += direction * 5f * (float)delta;
        return NodeState.RUNNING;
    }
}

public class Gather : GroveGames.BehaviourTree.Nodes.Node
{

    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        return NodeState.FAILURE;
    }
}

public class LookForGathering : GroveGames.BehaviourTree.Nodes.Node
{
    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
        return NodeState.FAILURE;
    }
}

public class LookForEnemy : GroveGames.BehaviourTree.Nodes.Node
{
	private readonly Node3D _enemy;
	private readonly Node3D _entity;

    public LookForEnemy(Node3D enemy, Node3D entity) : base()
    {
		_enemy = enemy;
		_entity = entity;
    }

    public override NodeState Evaluate(IBlackboard blackboard, double delta)
    {
		if (_enemy == null)
		{
			return NodeState.FAILURE;
		}

        var distance = _enemy.Position.DistanceTo(_entity.Position);

		if (distance <= 10f)
		{
            GD.Print("In Range");
			blackboard.SetValue("target_pos_enemey", _enemy);
			return NodeState.SUCCESS;
		}
		
        GD.Print("Out Of Range");
		return NodeState.FAILURE;
		
    }
}
