using System.Collections;

namespace Cubusky.Stats.Generators;

/// <summary>A thin wrapper around an array that provides structural (element-by-element) equality, so it can safely be used as part of an incremental generator's cached pipeline values (e.g. inside a record/record struct) without defeating caching.</summary>
internal readonly struct EquatableArray<T>(T[]? array) : IEquatable<EquatableArray<T>>, IReadOnlyList<T>
    where T : IEquatable<T>
{
    private readonly T[] _array = array ?? [];

    public static readonly EquatableArray<T> Empty = new([]);

    public int Count => _array.Length;

    public T this[int index] => _array[index];

    public bool Equals(EquatableArray<T> other) => _array.AsSpan().SequenceEqual(other._array.AsSpan());

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var item in _array)
        {
            hashCode.Add(item);
        }
        return hashCode.ToHashCode();
    }

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_array).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _array.GetEnumerator();

    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) => left.Equals(right);

    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) => !left.Equals(right);

    public static implicit operator EquatableArray<T>(T[] array) => new(array);
}
