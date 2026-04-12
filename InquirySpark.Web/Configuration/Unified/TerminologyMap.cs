namespace InquirySpark.Web.Configuration.Unified;

/// <summary>
/// Provides canonical unified terminology for user-visible labels.
/// Maps legacy DecisionSpark and InquirySpark.Admin terms to the single canonical term
/// used throughout the unified InquirySpark.Web UI.
/// Consult <c>contracts/unified-ux-conventions.md §5</c> for the authoritative table.
/// </summary>
public static class TerminologyMap
{
    /// <summary>
    /// Key: legacy term (case-insensitive). Value: canonical unified term.
    /// </summary>
    private static readonly IReadOnlyDictionary<string, string> _map =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // DecisionSpark → Unified
            ["Conversation"] = "Decision Conversation",
            ["Decision Conversation"] = "Decision Conversation",
            ["Spec"] = "Decision Specification",
            ["Decision Spec"] = "Decision Specification",
            ["DecisionSpec"] = "Decision Specification",
            ["Decision Specification"] = "Decision Specification",

            // InquirySpark.Admin → Unified
            ["App"] = "Application",
            ["Application"] = "Application",
            ["Survey Email"] = "Survey Email Template",
            ["Email Template"] = "Survey Email Template",
            ["Group Member"] = "Question Group Member",
            ["Group Members"] = "Question Group Members",
            ["Answer"] = "Question Answer",
            ["LuApplicationType"] = "Application Type",
            ["Application Type"] = "Application Type",
            ["LuQuestionType"] = "Question Type",
            ["Question Type"] = "Question Type",
            ["LuReviewStatus"] = "Review Status",
            ["Review Status"] = "Review Status",
            ["LuSurveyResponseStatus"] = "Survey Response Status",
            ["Survey Response Status"] = "Survey Response Status",
            ["LuSurveyType"] = "Survey Type",
            ["Survey Type"] = "Survey Type",
            ["LuUnitOfMeasure"] = "Unit of Measure",
            ["Unit of Measure"] = "Unit of Measure",

            // Operations Support
            ["Chart Definition"] = "Chart Definition",
            ["Chart Setting"] = "Chart Setting",
            ["User Preference"] = "User Preference",
        };

    /// <summary>
    /// Returns the canonical unified term for a given legacy/alternative term.
    /// If the term is not found in the map, the original value is returned unchanged.
    /// </summary>
    /// <param name="legacyTerm">The legacy or alternative term to look up.</param>
    /// <returns>The canonical unified term, or <paramref name="legacyTerm"/> if not mapped.</returns>
    public static string Resolve(string legacyTerm) =>
        _map.TryGetValue(legacyTerm, out var canonical) ? canonical : legacyTerm;

    /// <summary>
    /// Returns true if the given term is the canonical form (i.e., unchanged after Resolve).
    /// </summary>
    public static bool IsCanonical(string term) =>
        _map.TryGetValue(term, out var canonical) && canonical == term;
}
