using Godot;

using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree;

public class VariantBlackboard : IBlackboard
{
    private readonly Dictionary<string, Variant> _dataBase;

    public VariantBlackboard()
    {
        _dataBase = [];
    }

    public void Clear()
    {
        _dataBase.Clear();
    }

    public void DeleteValue(string key)
    {
        _dataBase.Remove(key);
    }

    public T? GetValue<T>(string key)
    {
        return _dataBase.TryGetValue(key, out var value) ? value.As<T>() : default(T);
    }

    public void SetValue<T>(string key, T obj) where T : notnull
    {
        _dataBase[key] = Variant.From(obj);
    }
}
