namespace GroveGames.BehaviourTree.Collections;

public class Blackboard : IBlackboard
{
    private readonly Dictionary<string, object> _database;

    public Blackboard()
    {
        _database = [];
    }

    public void Clear()
    {
        _database.Clear();
    }

    public void DeleteValue(string key)
    {
        _database.Remove(key);
    }

    public T? GetValue<T>(string key)
    {
        return _database.TryGetValue(key, out var value) ? (T)value : default;
    }

    public void SetValue<T>(string key, T obj) where T : notnull
    {
        _database[key] = obj;
    }
}
