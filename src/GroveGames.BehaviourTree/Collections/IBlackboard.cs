namespace GroveGames.BehaviourTree.Collections;

public interface IBlackboard
{
    void SetValue<T>(string key, T obj) where T : notnull;
    T? GetValue<T>(string key);
    void DeleteValue(string key);
    void Clear();
}
