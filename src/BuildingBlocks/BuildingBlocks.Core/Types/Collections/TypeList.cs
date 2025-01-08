using System.Collections;
using System.Reflection;
using BuildingBlocks.Abstractions.Types;

namespace BuildingBlocks.Core.Types.Collections;

/// <summary>
/// A shortcut for <see cref="TypeList{TBaseType}"/> to use object as the base type.
/// </summary>
public class TypeList : TypeList<object>, ITypeList
{
}

/// <summary>
/// Extends <see cref="List{T}"/> to add a restriction for a specific base type.
/// </summary>
/// <typeparam name="TBaseType">The base type of <see cref="Type"/>s in this list.</typeparam>
public class TypeList<TBaseType> : ITypeList<TBaseType>
{
    private readonly List<Type> _typeList;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeList{TBaseType}"/> class.
    /// </summary>
    public TypeList()
    {
        _typeList = [];
    }

    /// <summary>
    /// Gets the count of elements in the list.
    /// </summary>
    public int Count => _typeList.Count;

    /// <summary>
    /// Gets a value indicating whether this instance is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets or sets the <see cref="Type"/> at the specified index.
    /// </summary>
    /// <param name="index">The index of the element to get or set.</param>
    /// <returns>The <see cref="Type"/> at the specified index.</returns>
    public Type this[int index]
    {
        get => _typeList[index];
        set
        {
            CheckType(value);
            _typeList[index] = value;
        }
    }

    /// <summary>
    /// Adds a type to the list.
    /// </summary>
    /// <typeparam name="T">The type to add.</typeparam>
    public void Add<T>() where T : TBaseType
    {
        _typeList.Add(typeof(T));
    }

    /// <summary>
    /// Tries to add a type to the list if it doesn't already exist.
    /// </summary>
    /// <typeparam name="T">The type to add.</typeparam>
    /// <returns><c>true</c> if the type was added; otherwise, <c>false</c>.</returns>
    public bool TryAdd<T>() where T : TBaseType
    {
        if (Contains<T>())
        {
            return false;
        }

        Add<T>();
        return true;
    }

    /// <summary>
    /// Adds a type to the list.
    /// </summary>
    /// <param name="item">The type to add.</param>
    public void Add(Type item)
    {
        CheckType(item);
        _typeList.Add(item);
    }

    /// <summary>
    /// Inserts a type at the specified index.
    /// </summary>
    /// <param name="index">The index at which to insert the type.</param>
    /// <param name="item">The type to insert.</param>
    public void Insert(int index, Type item)
    {
        CheckType(item);
        _typeList.Insert(index, item);
    }

    /// <summary>
    /// Returns the index of the specified type.
    /// </summary>
    /// <param name="item">The type to locate in the list.</param>
    /// <returns>The index of the type if found; otherwise, -1.</returns>
    public int IndexOf(Type item)
    {
        return _typeList.IndexOf(item);
    }

    /// <summary>
    /// Checks if the list contains the specified type.
    /// </summary>
    /// <typeparam name="T">The type to check.</typeparam>
    /// <returns><c>true</c> if the list contains the type; otherwise, <c>false</c>.</returns>
    public bool Contains<T>() where T : TBaseType
    {
        return Contains(typeof(T));
    }

    /// <summary>
    /// Checks if the list contains the specified type.
    /// </summary>
    /// <param name="item">The type to check.</param>
    /// <returns><c>true</c> if the list contains the type; otherwise, <c>false</c>.</returns>
    public bool Contains(Type item)
    {
        return _typeList.Contains(item);
    }

    /// <summary>
    /// Removes the specified type from the list.
    /// </summary>
    /// <typeparam name="T">The type to remove.</typeparam>
    public void Remove<T>() where T : TBaseType
    {
        _typeList.Remove(typeof(T));
    }

    /// <summary>
    /// Removes the specified type from the list.
    /// </summary>
    /// <param name="item">The type to remove.</param>
    /// <returns><c>true</c> if the type was removed; otherwise, <c>false</c>.</returns>
    public bool Remove(Type item)
    {
        return _typeList.Remove(item);
    }

    /// <summary>
    /// Removes the type at the specified index.
    /// </summary>
    /// <param name="index">The index of the type to remove.</param>
    public void RemoveAt(int index)
    {
        _typeList.RemoveAt(index);
    }

    /// <summary>
    /// Clears all types from the list.
    /// </summary>
    public void Clear()
    {
        _typeList.Clear();
    }

    /// <summary>
    /// Copies the elements of the list to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The array to copy the elements to.</param>
    /// <param name="arrayIndex">The index in the array at which to begin copying.</param>
    public void CopyTo(Type[] array, int arrayIndex)
    {
        _typeList.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Returns an enumerator for iterating through the list.
    /// </summary>
    /// <returns>An enumerator for iterating through the list.</returns>
    public IEnumerator<Type> GetEnumerator()
    {
        return _typeList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _typeList.GetEnumerator();
    }

    /// <summary>
    /// Validates that a type is assignable to the base type.
    /// </summary>
    /// <param name="item">The type to check.</param>
    /// <exception cref="ArgumentException">Thrown when the type is not assignable to the base type.</exception>
    private static void CheckType(Type item)
    {
        if (!typeof(TBaseType).GetTypeInfo().IsAssignableFrom(item))
        {
            throw new ArgumentException(
                $"Given type ({item.AssemblyQualifiedName}) should be instance of {typeof(TBaseType).AssemblyQualifiedName}",
                nameof(item)
            );
        }
    }
}