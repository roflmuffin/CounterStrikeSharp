namespace CounterStrikeSharp.API.Core;

/// <summary>
/// Why a Binding failed to resolve against the running CS2 build. Closed set, append only
/// (mirrors <c>bindings::FailureReason</c> in native; values cross the boundary by name).
/// </summary>
public enum BindingFailureReason
{
    /// <summary>The Binding resolved.</summary>
    None = 0,

    /// <summary>The key (or the section the Binding needs) is not in Gamedata.</summary>
    MissingFromGamedata = 1,

    /// <summary>The key is in Gamedata, but has no entry for this OS.</summary>
    MissingForThisOs = 2,

    /// <summary>The signature is not valid hex.</summary>
    InvalidPattern = 3,

    /// <summary>The signature matched nothing (or its module is not loaded).</summary>
    NotFound = 4,

    /// <summary>The signature matched more than once; see the match count.</summary>
    Ambiguous = 5,

    /// <summary>No vtable was found for the RTTI class name.</summary>
    RttiVtableNotFound = 6,

    /// <summary>The schema class does not exist.</summary>
    SchemaClassMissing = 7,

    /// <summary>The schema field does not exist.</summary>
    SchemaFieldMissing = 8,

    /// <summary>A Slot Binding's fingerprint window does not match the class's live vtable at its index.</summary>
    FingerprintMismatch = 9,

    /// <summary>
    /// A Slot Binding's fingerprint window does not match at its index and matches more than once nearby, so it is not
    /// healed; see the match count.
    /// </summary>
    AmbiguousHeal = 10,
}

internal static class BindingFailureReasonExtensions
{
    /// <summary>Same wording as native <c>bindings::DescribeFailure</c>, so logs read alike on both sides.</summary>
    public static string Describe(this BindingFailureReason reason, int matchCount) => reason switch
    {
        BindingFailureReason.None => "resolved",
        BindingFailureReason.MissingFromGamedata => "missing from gamedata",
        BindingFailureReason.MissingForThisOs => "missing for this OS",
        BindingFailureReason.InvalidPattern => "invalid pattern",
        BindingFailureReason.NotFound => "not found",
        BindingFailureReason.Ambiguous => $"ambiguous ({matchCount} matches)",
        BindingFailureReason.RttiVtableNotFound => "RTTI vtable not found",
        BindingFailureReason.SchemaClassMissing => "schema class missing",
        BindingFailureReason.SchemaFieldMissing => "schema field missing",
        BindingFailureReason.FingerprintMismatch => "fingerprint mismatch",
        BindingFailureReason.AmbiguousHeal => $"ambiguous heal ({matchCount} matches)",
        _ => "unknown",
    };
}
