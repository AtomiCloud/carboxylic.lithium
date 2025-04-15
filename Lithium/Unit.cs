namespace CarboxylicLithium;

/// <summary>
/// Empty Type to return for void functions to improve compatibility and code reuse for Results.
/// </summary>
public struct Unit
{
    /// <summary>
    /// All Unit types are equal and have no value.
    /// </summary>
    /// <param name="obj">object to check equality</param>
    /// <returns>True if and only if the object is of type Unit</returns>
    public readonly override bool Equals(object? obj)
    {
        return obj is Unit;
    }

    /// <summary>
    /// Unit has no string value, so it will return an empty string on ToString()
    /// </summary>
    /// <returns>Empty String</returns>
    public readonly override string ToString()
    {
        return string.Empty;
    }

#pragma warning disable IDE0060
    /// <summary>
    /// Unit has no value and consequently has a hashcode of 0
    /// </summary>
    /// <returns>0</returns>
    public readonly override int GetHashCode()
    {
        return 0;
    }

    /// <summary>
    /// All Unit are equal to each other, so this is always true.
    /// </summary>
    /// <param name="left">First Unit</param>
    /// <param name="right">Second Unit</param>
    /// <returns></returns>
    public static bool operator ==(Unit left, Unit right)
    {
        return true;
    }

    /// <summary>
    /// All Unit are equal to each other, so this is always false.
    /// </summary>
    /// <param name="left">First Unit</param>
    /// <param name="right">Second Unit</param>
    /// <returns></returns>
    public static bool operator !=(Unit left, Unit right)
    {
        return false;
    }
#pragma warning restore IDE0060
}
