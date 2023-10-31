namespace CounterStrikeSharp.API.Core
{
    /// <summary>
    /// Specifies the hook mode.
    /// </summary>
    public enum HookMode
    {
        /// <summary>
        /// The hook is called before the original method.
        /// </summary>
        Pre = 0,

        /// <summary>
        /// The hook is called after the original method.
        /// </summary>
        Post = 1,
    }
}