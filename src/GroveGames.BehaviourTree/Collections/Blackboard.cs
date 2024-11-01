using System;

namespace GroveGames.BehaviourTree.Collections;

public class Blackboard : IBlackboard
{
    private readonly Dictionary<string, object> _database;

    public Blackboard()
    {
        _database = [];
    }

    public void DeleteValue(string key)
    {
        _database.Remove(key);
    }

    public T? GetValue<T>(string key)
    {
        return _database.TryGetValue(key, out var value) ? (T)value : default(T);
    }

    public void SetValue<T>(string key, T obj) where T : notnull
    {
        _database.TryAdd(key, obj);
    }
}
