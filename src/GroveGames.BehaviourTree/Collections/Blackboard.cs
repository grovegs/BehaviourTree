namespace GroveGames.BehaviourTree.Collections;

public sealed class Blackboard : IBlackboard
{
    private interface IStore
    {
        public void Clear();
    }

    private sealed class Store<T> : IStore
    {
        public readonly Dictionary<string, T> Values;

        public Store()
        {
            Values = [];
        }

        public void Clear()
        {
            Values.Clear();
        }
    }

    private readonly Dictionary<Type, IStore> _valueStores;
    private readonly Dictionary<string, object> _references;

    public Blackboard()
    {
        _valueStores = [];
        _references = [];
    }

    public void SetValue<T>(BlackboardKey<T> key, T value)
    {
        if (typeof(T).IsValueType)
        {
            GetOrCreateStore<T>().Values[key.Name] = value;
            return;
        }

        _references[key.Name] = value!;
    }

    public bool TryGetValue<T>(BlackboardKey<T> key, out T value)
    {
        if (typeof(T).IsValueType)
        {
            if (_valueStores.TryGetValue(typeof(T), out var store))
            {
                return ((Store<T>)store).Values.TryGetValue(key.Name, out value!);
            }

            value = default!;
            return false;
        }

        if (_references.TryGetValue(key.Name, out var reference) && reference is T typed)
        {
            value = typed;
            return true;
        }

        value = default!;
        return false;
    }

    public void DeleteValue<T>(BlackboardKey<T> key)
    {
        if (typeof(T).IsValueType)
        {
            if (_valueStores.TryGetValue(typeof(T), out var store))
            {
                ((Store<T>)store).Values.Remove(key.Name);
            }

            return;
        }

        if (_references.TryGetValue(key.Name, out var reference) && reference is T)
        {
            _references.Remove(key.Name);
        }
    }

    public void Clear()
    {
        foreach (var store in _valueStores.Values)
        {
            store.Clear();
        }

        _references.Clear();
    }

    private Store<T> GetOrCreateStore<T>()
    {
        if (_valueStores.TryGetValue(typeof(T), out var store))
        {
            return (Store<T>)store;
        }

        var created = new Store<T>();
        _valueStores.Add(typeof(T), created);
        return created;
    }
}
