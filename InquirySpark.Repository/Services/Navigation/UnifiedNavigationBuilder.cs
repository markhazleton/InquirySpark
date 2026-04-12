using InquirySpark.Repository.Models.Navigation;

namespace InquirySpark.Repository.Services.Navigation;

/// <summary>
/// Builds the unified navigation tree for InquirySpark.Web.
/// Returns a two-level menu covering all inventoried capability families
/// sourced from DecisionSpark and InquirySpark.Admin.
/// </summary>
public sealed class UnifiedNavigationBuilder
{
    /// <summary>
    /// Builds the canonical navigation node list for the unified layout.
    /// </summary>
    public IReadOnlyList<UnifiedNavigationNodeViewModel> Build() =>
    [
        new()
        {
            Label    = "Home",
            Href     = "/Unified/Operations",
            Icon     = "bi-house-door",
            NavKey   = "/unified/operations",
            Order    = 0,
        },

        new()
        {
            Label    = "Decision Workspace",
            Icon     = "bi-diagram-3",
            NavKey   = "/unified/decision",
            IsGroup  = true,
            Order    = 10,
            Children =
            [
                new() { Label = "Conversations",           Href = "/Unified/DecisionConversation",           Icon = "bi-chat-dots",          NavKey = "/unified/decisionconversation",          Order = 1 },
                new() { Label = "Decision Specifications", Href = "/Unified/DecisionSpecification",          Icon = "bi-file-earmark-ruled",  NavKey = "/unified/decisionspecification",         Order = 2 },
            ],
        },

        new()
        {
            Label    = "Inquiry Administration",
            Icon     = "bi-gear",
            NavKey   = "/unified/inquiryadministration",
            IsGroup  = true,
            Order    = 20,
            Children =
            [
                new() { Label = "Applications",         Href = "/Unified/InquiryAdministration/Applications",         Icon = "bi-app-indicator",    NavKey = "/unified/inquiryadministration/applications",         Order = 1 },
                new() { Label = "Application Users",    Href = "/Unified/InquiryAdministration/ApplicationUsers",      Icon = "bi-people",           NavKey = "/unified/inquiryadministration/applicationusers",      Order = 2 },
                new() { Label = "User Roles",           Href = "/Unified/InquiryAdministration/ApplicationUserRoles",  Icon = "bi-person-badge",     NavKey = "/unified/inquiryadministration/applicationuserroles",  Order = 3 },
                new() { Label = "Application Surveys",  Href = "/Unified/InquiryAdministration/ApplicationSurveys",    Icon = "bi-clipboard-check",  NavKey = "/unified/inquiryadministration/applicationsurveys",    Order = 4 },
                new() { Label = "App Properties",       Href = "/Unified/InquiryAdministration/AppProperties",         Icon = "bi-sliders",          NavKey = "/unified/inquiryadministration/appproperties",         Order = 5 },
                new() { Label = "Roles",                Href = "/Unified/InquiryAdministration/Roles",                 Icon = "bi-shield-lock",      NavKey = "/unified/inquiryadministration/roles",                 Order = 6 },
            ],
        },

        new()
        {
            Label    = "Inquiry Authoring",
            Icon     = "bi-pencil-square",
            NavKey   = "/unified/inquiryauthoring",
            IsGroup  = true,
            Order    = 30,
            Children =
            [
                new() { Label = "Surveys",               Href = "/Unified/InquiryAuthoring/Surveys",              Icon = "bi-card-list",          NavKey = "/unified/inquiryauthoring/surveys",              Order = 1 },
                new() { Label = "Questions",             Href = "/Unified/InquiryAuthoring/Questions",            Icon = "bi-question-circle",    NavKey = "/unified/inquiryauthoring/questions",            Order = 2 },
                new() { Label = "Question Groups",       Href = "/Unified/InquiryAuthoring/QuestionGroups",       Icon = "bi-collection",         NavKey = "/unified/inquiryauthoring/questiongroups",       Order = 3 },
                new() { Label = "Group Members",         Href = "/Unified/InquiryAuthoring/QuestionGroupMembers", Icon = "bi-person-lines-fill",  NavKey = "/unified/inquiryauthoring/questiongroupmembers", Order = 4 },
                new() { Label = "Answers",               Href = "/Unified/InquiryAuthoring/QuestionAnswers",      Icon = "bi-check2-circle",      NavKey = "/unified/inquiryauthoring/questionanswers",      Order = 5 },
                new() { Label = "Email Templates",       Href = "/Unified/InquiryAuthoring/SurveyEmailTemplates", Icon = "bi-envelope",           NavKey = "/unified/inquiryauthoring/surveyemailtemplates", Order = 6 },
            ],
        },

        new()
        {
            Label    = "Inquiry Operations",
            Icon     = "bi-activity",
            NavKey   = "/unified/inquiryoperations",
            IsGroup  = true,
            Order    = 40,
            Children =
            [
                new() { Label = "Companies",          Href = "/Unified/InquiryOperations/Companies",          Icon = "bi-building",        NavKey = "/unified/inquiryoperations/companies",          Order = 1 },
                new() { Label = "Import History",     Href = "/Unified/InquiryOperations/ImportHistories",    Icon = "bi-cloud-upload",    NavKey = "/unified/inquiryoperations/importhistories",    Order = 2 },
                new() { Label = "Survey Status",      Href = "/Unified/InquiryOperations/SurveyStatus",       Icon = "bi-toggle-on",       NavKey = "/unified/inquiryoperations/surveystatus",       Order = 3 },
                new() { Label = "Review Status",      Href = "/Unified/InquiryOperations/SurveyReviewStatus", Icon = "bi-check-all",       NavKey = "/unified/inquiryoperations/surveyreviewstatus", Order = 4 },
                new() { Label = "Site Roles",         Href = "/Unified/InquiryOperations/SiteRoles",          Icon = "bi-key",             NavKey = "/unified/inquiryoperations/siteroles",          Order = 5 },
                new() { Label = "Site Menus",         Href = "/Unified/InquiryOperations/SiteAppMenus",       Icon = "bi-layout-three-columns", NavKey = "/unified/inquiryoperations/siteappmenus",  Order = 6 },
            ],
        },

        new()
        {
            Label    = "Operations Support",
            Icon     = "bi-tools",
            NavKey   = "/unified/operationssupport",
            IsGroup  = true,
            Order    = 50,
            Children =
            [
                new() { Label = "System Health",      Href = "/Unified/OperationsSupport/Health",          Icon = "bi-heart-pulse",    NavKey = "/unified/operationssupport/health",          Order = 1 },
                new() { Label = "Chart Builder",      Href = "/Unified/OperationsSupport/ChartBuilder",    Icon = "bi-bar-chart-line", NavKey = "/unified/operationssupport/chartbuilder",    Order = 2 },
                new() { Label = "User Preferences",   Href = "/Unified/OperationsSupport/UserPreferences", Icon = "bi-person-gear",    NavKey = "/unified/operationssupport/userpreferences", Order = 3 },
            ],
        },

        new()
        {
            Label    = "Capability Matrix",
            Href     = "/Unified/CapabilityCompletionMatrix",
            Icon     = "bi-kanban",
            NavKey   = "/unified/capabilitycompletionmatrix",
            Order    = 60,
        },
    ];
}
