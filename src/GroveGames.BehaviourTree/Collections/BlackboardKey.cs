namespace GroveGames.BehaviourTree.Collections;

public readonly struct BlackboardKey<T> : IEquatable<BlackboardKey<T>>
{
    private readonly string _name;

    public string Name => _name;

    public BlackboardKey(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        _name = name;
    }

    public bool Equals(BlackboardKey<T> other)
    {
        return _name == other._name;
    }

    public override bool Equals(object? obj)
    {
        return obj is BlackboardKey<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _name.GetHashCode();
    }

    public override string ToString()
    {
        return _name;
    }

    public static bool operator ==(BlackboardKey<T> left, BlackboardKey<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BlackboardKey<T> left, BlackboardKey<T> right)
    {
        return !left.Equals(right);
    }
}
