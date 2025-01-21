using Godot;

using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree;

public class VariantBlackboard : IBlackboard
{
    private readonly Dictionary<string, Variant> _database;

    public VariantBlackboard()
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
        return _database.TryGetValue(key, out var value) ? value.As<T>() : default(T);
    }

    public void SetValue<T>(string key, T obj) where T : notnull
    {
        _database[key] = Variant.From(obj);
    }
}
