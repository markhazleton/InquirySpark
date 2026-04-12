namespace InquirySpark.Repository.Configuration.Unified;

/// <summary>
/// Encodes the canonical URL route policy for the unified InquirySpark area.
/// All user-facing capability surfaces are rooted under /Unified/.
/// </summary>
public static class CanonicalRoutePolicy
{
    /// <summary>The area name used by all unified capability controllers.</summary>
    public const string AreaName = "Unified";

    /// <summary>The URL prefix for all unified capability routes.</summary>
    public const string UrlPrefix = "/Unified";

    /// <summary>Returns true if the supplied path matches the canonical unified area prefix.</summary>
    public static bool IsUnifiedRoute(string? path) =>
        path is not null &&
        path.StartsWith(UrlPrefix, StringComparison.OrdinalIgnoreCase);

    /// <summary>Policy name constants matching authorization policy definitions in Program.cs.</summary>
    public static class Policies
    {
        /// <summary>Analyst-level access (Analyst or Administrator).</summary>
        public const string Analyst = "Analyst";

        /// <summary>Operator-level access (Operator or Administrator).</summary>
        public const string Operator = "Operator";

        /// <summary>Consultant-level access (Consultant or Administrator).</summary>
        public const string Consultant = "Consultant";

        /// <summary>Executive-level access (Executive or Administrator).</summary>
        public const string Executive = "Executive";
    }
}
