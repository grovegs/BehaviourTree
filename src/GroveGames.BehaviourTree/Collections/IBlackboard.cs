namespace GroveGames.BehaviourTree.Collections;

public interface IBlackboard
{
    public void SetValue<T>(BlackboardKey<T> key, T value);
    public bool TryGetValue<T>(BlackboardKey<T> key, out T value);
    public void DeleteValue<T>(BlackboardKey<T> key);
    public void Clear();
}
