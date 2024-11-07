# GroveGames.BehaviourTree

[![Build Status](https://github.com/grovegs/BehaviourTree/actions/workflows/release.yml/badge.svg)](https://github.com/grovegs/BehaviourTree/actions/workflows/release.yml)
[![Tests](https://github.com/grovegs/BehaviourTree/actions/workflows/tests.yml/badge.svg)](https://github.com/grovegs/BehaviourTree/actions/workflows/tests.yml)
[![Latest Release](https://img.shields.io/github/v/release/grovegs/BehaviourTree)](https://github.com/grovegs/BehaviourTree/releases/latest)
[![NuGet](https://img.shields.io/nuget/v/GroveGames.BehaviourTree)](https://www.nuget.org/packages/GroveGames.BehaviourTree)

A lightweight behaviour tree framework developed by Grove Games for .NET and Godot.

## Installation

Use nuget to add you project.

```bash
dotnet add package GroveGames.BehaviourTree.Godot
```

## Usage

You can derive from the Node class to create a Custom Node.

```csharp
public class Attack : Node
{
    public Attack(IParent parent) : base(parent)
    {
    }

    public override NodeState Evaluate(float delta)
    {
        Console.WriteLine("Attacking");
        return NodeState.Success;
    }
}
```
Derive a class from the `Tree` class to create a tree structure.

```csharp
public class CharacterBehaviourTree : Tree
{
    public CharacterBehaviourTree(Root root) : base(root)
    {
    }

    public override void SetupTree()
    {  
    }
}
```
There are extensions available to help you create nodes easily.

```csharp
    public override void SetupTree()
    {  
        var selector = Root.Selector();
        
        var attackSequence =  selector.Sequence();
        attackSequence.Attach(new Condition(() => IsEnemyVisible()));

        attackSequence
        .Cooldown(1f)
        .Repeater(RepeatMode.UntilSuccess)
        .Inverter()
        .Attach(new Attack(attackSequence));

        var defendSequence = selector.Sequence()
        defendSequence.Attach(new Condition(() => IsUnderAttack()));
        defendSequence
        .Cooldown(1f)
        .Repeater(RepeatMode.UntilSuccess)
        .Inverter()
        .Attach(new Defend(defenceSequence));
    }
```
Here how it looks like
```csharp
Root (Selector)
├── Sequence (Attack Sequence)
│   ├── Condition (IsEnemyVisible == true)
│   ├── Cooldown
│   │   └── Repeater
│   │       └── Inverter
│   │           └── Attack
│   
└── Sequence (Defend Sequence)
    ├── Condition (IsUnderAttack == true)
    ├── Cooldown
    │   └── Repeater
    │       └── Inverter
    │           └── Defend


```
## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
