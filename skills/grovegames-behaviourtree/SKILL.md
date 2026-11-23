---
name: grovegames-behaviourtree
description: Rules for AI behaviour trees in game code with GroveGames.BehaviourTree. Use when writing or reviewing a behaviour tree, a tree node, blackboard keys, or code that ticks a tree.
---

# Using GroveGames.BehaviourTree

Rules for game code that uses this package. They hold in every game. The API is in the package's `README.md`.

- Describe a tree in `SetupTree` with the fluent builder (`Selector`, `Sequence`, `Parallel`, decorators). Do not wire nodes together by hand.
- One node does one thing. Inherit from `BehaviourNode`, implement `Evaluate`, and always set and return a `NodeState`: `Running` while the action continues, `Success` or `Failure` when it ends.
- Nodes share state only through the blackboard. Define every `BlackboardKey<T>` once as a static field, and read with `TryGetValue`.
- A node must not block or wait; return `Running` and continue on the next tick. Use `Cooldown` and `Delayer` for timing.
- Put a branch that is used more than once in a `ChildTree`.
- Keep game rules in services; a node calls a service and reports the result.
