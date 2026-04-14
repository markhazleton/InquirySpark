namespace InquirySpark.Web.Configuration.Unified;

/// <summary>
/// Maps canonical capability identifiers to their unified route patterns.
/// Used to resolve cross-domain navigation and parity verification.
/// </summary>
public sealed class CapabilityRoutingMap
{
    /// <summary>
    /// Returns the full capability-to-route mapping for all unified capability surfaces.
    /// Key: CapabilityId (e.g., "CAP-DS-001"). Value: relative URL pattern.
    /// </summary>
    public static IReadOnlyDictionary<string, string> Entries { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // ── DecisionSpark domain ───────────────────────────────────────────
        ["CAP-DS-001"] = "/Unified/DecisionConversation",
        ["CAP-DS-002"] = "/Unified/DecisionSpecification",
        ["CAP-DS-003"] = "/Unified/DecisionSpecification",
        ["CAP-DS-004"] = "/Unified/DecisionSpecification/Draft",
        ["CAP-DS-005"] = "/Unified/DecisionSpecification/{id}/Status",
        ["CAP-DS-006"] = "/Unified/DecisionSpecification/{id}/Audit",
        ["CAP-DS-007"] = "/Unified/OperationsSupport/Health",

        // ── InquirySpark.Admin — Administration domain ─────────────────────
        ["CAP-IA-001"] = "/Unified/InquiryAdministration/Applications",
        ["CAP-IA-002"] = "/Unified/InquiryAdministration/ApplicationUsers",
        ["CAP-IA-003"] = "/Unified/InquiryAdministration/ApplicationUserRoles",
        ["CAP-IA-004"] = "/Unified/InquiryAdministration/ApplicationSurveys",
        ["CAP-IA-005"] = "/Unified/InquiryAdministration/AppProperties",
        ["CAP-IA-024"] = "/Unified/InquiryAdministration/Roles",

        // ── InquirySpark.Admin — Authoring domain ──────────────────────────
        ["CAP-IA-006"] = "/Unified/InquiryAuthoring/Surveys",
        ["CAP-IA-007"] = "/Unified/InquiryAuthoring/SurveyEmailTemplates",
        ["CAP-IA-008"] = "/Unified/InquiryAuthoring/Questions",
        ["CAP-IA-009"] = "/Unified/InquiryAuthoring/QuestionGroups",
        ["CAP-IA-010"] = "/Unified/InquiryAuthoring/QuestionGroupMembers",
        ["CAP-IA-011"] = "/Unified/InquiryAuthoring/QuestionAnswers",

        // ── InquirySpark.Admin — Operations domain ─────────────────────────
        ["CAP-IA-012"] = "/Unified/InquiryOperations/Companies",
        ["CAP-IA-013"] = "/Unified/InquiryOperations/ImportHistories",
        ["CAP-IA-014"] = "/Unified/InquiryOperations/SurveyStatus",
        ["CAP-IA-015"] = "/Unified/InquiryOperations/SurveyReviewStatus",
        ["CAP-IA-016"] = "/Unified/InquiryOperations/SiteRoles",
        ["CAP-IA-017"] = "/Unified/InquiryOperations/SiteAppMenus",

        // ── InquirySpark.Admin — Lookup domain ─────────────────────────────
        ["CAP-IA-018"] = "/Unified/InquiryAdministration/LuApplicationTypes",
        ["CAP-IA-019"] = "/Unified/InquiryAdministration/LuQuestionTypes",
        ["CAP-IA-020"] = "/Unified/InquiryAdministration/LuReviewStatus",
        ["CAP-IA-021"] = "/Unified/InquiryAdministration/LuSurveyResponseStatus",
        ["CAP-IA-022"] = "/Unified/InquiryAdministration/LuSurveyTypes",
        ["CAP-IA-023"] = "/Unified/InquiryAdministration/LuUnitOfMeasures",

        // ── InquirySpark.Admin — Operations Support domain ─────────────────
        ["CAP-IA-025"] = "/Unified/OperationsSupport/ChartSettings",
        ["CAP-IA-026"] = "/Unified/OperationsSupport/ChartBuilder",
        ["CAP-IA-027"] = "/Unified/OperationsSupport/ChartDefinitions",
        ["CAP-IA-028"] = "/Unified/OperationsSupport/SystemHealth",
        ["CAP-IA-029"] = "/Unified/OperationsSupport/UserPreferences",
        ["CAP-IA-030"] = "/Unified/DecisionConversation/Api",
    };

    /// <summary>Resolves the unified route for a given capability identifier.</summary>
    /// <param name="capabilityId">The capability identifier (e.g., "CAP-DS-001").</param>
    /// <returns>The unified route or null if not found.</returns>
    public static string? Resolve(string capabilityId) =>
        Entries.TryGetValue(capabilityId, out var route) ? route : null;
}
