namespace BlogDotNet.DataServices.Abstractions.Models;

/// <summary>
/// A listing of <typeparamref name="T"/>
/// </summary>
/// <typeparam name="T">Items type</typeparam>
public class Listing<T>
{
    /// <summary>
    /// Parameter for next page (items after this)
    /// </summary>
    public Guid? After { get; set; } = null;

    /// <summary>
    /// Parameter for previous page (items before this)
    /// </summary>
    public Guid? Before { get; set; } = null;

    /// <summary>
    /// Items in the listing
    /// </summary>
    public List<T> Items { get; set; } = [];
}
