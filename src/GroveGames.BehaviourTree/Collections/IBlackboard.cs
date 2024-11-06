namespace GroveGames.BehaviourTree.Collections;

public interface IBlackboard
{
    public void SetValue<T>(string key, T obj) where T : notnull;
    public T? GetValue<T>(string key);
    public void DeleteValue(string key);
}
