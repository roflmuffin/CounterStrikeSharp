namespace CounterStrikeSharp.SchemaGen;

public enum SchemaAtomicCategory
{
    /// <summary>
    /// Engine primitive value (e.g. Color, Vector, CUtlSymbolLarge, CUtlString)
    /// </summary>
    Basic = 0,
    /// <summary>
    /// Generic/templated type (e.g. CHandle, CWeakHandle)
    /// </summary>
    T = 1,
    /// <summary>
    /// e.g. CNetworkUtlVectorBase, CUtlVectorEmbeddedNetworkVar, CUtlVector
    /// </summary>
    Collection = 2,
    /// <summary>
    /// Not seen in CS2
    /// </summary>
    TT = 3,
    /// <summary>
    /// Not seen in CS2
    /// </summary>
    I = 4,
    /// <summary>
    /// Not seen in CS2
    /// </summary>
    Unknown = 5,
    None = 6
};
