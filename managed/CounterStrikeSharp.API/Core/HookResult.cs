namespace CounterStrikeSharp.API.Core
{
    /// <summary>
    /// Represents the result of a hook.
    /// </summary>
    public enum HookResult
    {
        /// <summary>
        /// Continue processing the hook to other listeners.
        /// </summary>
        Continue = 0,

        /// <summary>
        /// The hook result has been changed. Currently unused.
        /// </summary>
        Changed = 1,

        /// <summary>
        /// The hook has been handled. The original method will not be called, but other hooks will still be called.
        /// </summary>
        Handled = 3,

        /// <summary>
        /// Stop processing the hook. The original method will not be called, and other hooks will not proceed after this one.
        /// </summary>
        Stop = 4,
    }
}