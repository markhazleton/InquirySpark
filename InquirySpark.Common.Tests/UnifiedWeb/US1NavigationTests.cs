#nullable enable
using InquirySpark.Repository.Configuration.Unified;
using InquirySpark.Repository.Services.Navigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InquirySpark.Common.Tests.UnifiedWeb;

/// <summary>
/// Unit tests for UnifiedNavigationBuilder, OperationsController route coverage,
/// and capability-family routing map for US1 parity (T030A).
/// </summary>
[TestClass]
public class US1NavigationTests
{
    private static readonly UnifiedNavigationBuilder _builder = new();

    // ── UnifiedNavigationBuilder ──────────────────────────────────────────

    [TestMethod]
    public void Build_ReturnsNonEmptyNodeList()
    {
        var nodes = _builder.Build();
        Assert.IsTrue(nodes.Count > 0, "Navigation builder must return at least one root node.");
    }

    [TestMethod]
    public void Build_ContainsAllCapabilityFamilyGroups()
    {
        var nodes = _builder.Build();
        var labels = nodes.Select(n => n.Label).ToList();

        Assert.IsTrue(labels.Contains("Home"), "Home node must be present.");
        Assert.IsTrue(labels.Contains("Decision Workspace"), "Decision Workspace group must be present.");
        Assert.IsTrue(labels.Contains("Inquiry Administration"), "Inquiry Administration group must be present.");
        Assert.IsTrue(labels.Contains("Inquiry Authoring"), "Inquiry Authoring group must be present.");
        Assert.IsTrue(labels.Contains("Inquiry Operations"), "Inquiry Operations group must be present.");
        Assert.IsTrue(labels.Contains("Operations Support"), "Operations Support group must be present.");
        Assert.IsTrue(labels.Contains("Capability Matrix"), "Capability Matrix node must be present.");
    }

    [TestMethod]
    public void Build_DecisionWorkspaceGroup_ContainsConversationsAndSpecifications()
    {
        var nodes = _builder.Build();
        var decisionGroup = nodes.FirstOrDefault(n => n.Label == "Decision Workspace");

        Assert.IsNotNull(decisionGroup, "Decision Workspace group must exist.");
        Assert.IsTrue(decisionGroup.IsGroup, "Decision Workspace must be a group node.");

        var childLabels = decisionGroup.Children.Select(c => c.Label).ToList();
        Assert.IsTrue(childLabels.Contains("Conversations"), "Decision Workspace must include Conversations.");
        Assert.IsTrue(childLabels.Contains("Decision Specifications"), "Decision Workspace must include Decision Specifications.");
    }

    [TestMethod]
    public void Build_InquiryAdministrationGroup_ContainsAllRequiredChildren()
    {
        var nodes = _builder.Build();
        var adminGroup = nodes.FirstOrDefault(n => n.Label == "Inquiry Administration");

        Assert.IsNotNull(adminGroup, "Inquiry Administration group must exist.");
        var childLabels = adminGroup.Children.Select(c => c.Label).ToList();

        Assert.IsTrue(childLabels.Contains("Applications"), "Must include Applications.");
        Assert.IsTrue(childLabels.Contains("Application Users"), "Must include Application Users.");
        Assert.IsTrue(childLabels.Contains("User Roles"), "Must include User Roles.");
        Assert.IsTrue(childLabels.Contains("Application Surveys"), "Must include Application Surveys.");
        Assert.IsTrue(childLabels.Contains("App Properties"), "Must include App Properties.");
        Assert.IsTrue(childLabels.Contains("Roles"), "Must include Roles.");
    }

    [TestMethod]
    public void Build_InquiryAuthoringGroup_ContainsAllRequiredChildren()
    {
        var nodes = _builder.Build();
        var authoringGroup = nodes.FirstOrDefault(n => n.Label == "Inquiry Authoring");

        Assert.IsNotNull(authoringGroup);
        var childLabels = authoringGroup.Children.Select(c => c.Label).ToList();

        Assert.IsTrue(childLabels.Contains("Surveys"));
        Assert.IsTrue(childLabels.Contains("Questions"));
        Assert.IsTrue(childLabels.Contains("Question Groups"));
        Assert.IsTrue(childLabels.Contains("Group Members"));
        Assert.IsTrue(childLabels.Contains("Answers"));
        Assert.IsTrue(childLabels.Contains("Email Templates"));
    }

    [TestMethod]
    public void Build_InquiryOperationsGroup_ContainsAllRequiredChildren()
    {
        var nodes = _builder.Build();
        var opsGroup = nodes.FirstOrDefault(n => n.Label == "Inquiry Operations");

        Assert.IsNotNull(opsGroup);
        var childLabels = opsGroup.Children.Select(c => c.Label).ToList();

        Assert.IsTrue(childLabels.Contains("Companies"));
        Assert.IsTrue(childLabels.Contains("Import History"));
        Assert.IsTrue(childLabels.Contains("Survey Status"));
        Assert.IsTrue(childLabels.Contains("Review Status"));
        Assert.IsTrue(childLabels.Contains("Site Roles"));
        Assert.IsTrue(childLabels.Contains("Site Menus"));
    }

    [TestMethod]
    public void Build_OperationsSupportGroup_ContainsRequiredChildren()
    {
        var nodes = _builder.Build();
        var supportGroup = nodes.FirstOrDefault(n => n.Label == "Operations Support");

        Assert.IsNotNull(supportGroup);
        var childLabels = supportGroup.Children.Select(c => c.Label).ToList();

        Assert.IsTrue(childLabels.Contains("System Health"), "System Health must be present.");
        Assert.IsTrue(childLabels.Contains("Chart Builder"), "Chart Builder must be present.");
        Assert.IsTrue(childLabels.Contains("User Preferences"), "User Preferences must be present.");
    }

    [TestMethod]
    public void Build_AllChildNodesHaveNonEmptyHref()
    {
        var nodes = _builder.Build();
        foreach (var node in nodes)
        {
            if (!node.IsGroup)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(node.Href),
                    $"Non-group node '{node.Label}' must have a non-empty Href.");
            }
            foreach (var child in node.Children)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(child.Href),
                    $"Child node '{child.Label}' under '{node.Label}' must have a non-empty Href.");
            }
        }
    }

    [TestMethod]
    public void Build_AllHrefsStartWithUnifiedPrefix()
    {
        var nodes = _builder.Build();
        foreach (var node in nodes)
        {
            if (!node.IsGroup && !string.IsNullOrWhiteSpace(node.Href))
            {
                Assert.IsTrue(CanonicalRoutePolicy.IsUnifiedRoute(node.Href),
                    $"Root node '{node.Label}' href '{node.Href}' must be under /Unified/.");
            }
            foreach (var child in node.Children)
            {
                Assert.IsTrue(CanonicalRoutePolicy.IsUnifiedRoute(child.Href),
                    $"Child node '{child.Label}' href '{child.Href}' must be under /Unified/.");
            }
        }
    }

    // ── CapabilityRoutingMap ──────────────────────────────────────────────

    [TestMethod]
    public void CapabilityRoutingMap_ContainsAllInventoriedCapabilityIds()
    {
        var expectedIds = new[]
        {
            "CAP-DS-001", "CAP-DS-002", "CAP-DS-003", "CAP-DS-004",
            "CAP-DS-005", "CAP-DS-006", "CAP-DS-007",
            "CAP-IA-001", "CAP-IA-002", "CAP-IA-003", "CAP-IA-004",
            "CAP-IA-005", "CAP-IA-006", "CAP-IA-007", "CAP-IA-008",
            "CAP-IA-009", "CAP-IA-010", "CAP-IA-011", "CAP-IA-012",
            "CAP-IA-013", "CAP-IA-014", "CAP-IA-015", "CAP-IA-016",
            "CAP-IA-017", "CAP-IA-024",
        };

        foreach (var id in expectedIds)
        {
            var route = CapabilityRoutingMap.Resolve(id);
            Assert.IsNotNull(route, $"Capability '{id}' must have a unified route mapping.");
            Assert.IsTrue(CanonicalRoutePolicy.IsUnifiedRoute(route),
                $"Route for '{id}' ('{route}') must be under /Unified/.");
        }
    }

    [TestMethod]
    public void CapabilityRoutingMap_UnknownId_ReturnsNull()
    {
        var route = CapabilityRoutingMap.Resolve("CAP-UNKNOWN-999");
        Assert.IsNull(route, "Unknown capability ID must return null.");
    }

    [TestMethod]
    public void CapabilityRoutingMap_CaseInsensitiveLookup()
    {
        var lower = CapabilityRoutingMap.Resolve("cap-ds-001");
        var upper = CapabilityRoutingMap.Resolve("CAP-DS-001");
        Assert.AreEqual(upper, lower, "Capability routing map lookup must be case-insensitive.");
    }

    // ── CanonicalRoutePolicy ──────────────────────────────────────────────

    [TestMethod]
    [DataRow("/Unified/Operations")]
    [DataRow("/Unified/DecisionConversation")]
    [DataRow("/unified/inquiryadministration/applications")]
    public void CanonicalRoutePolicy_IsUnifiedRoute_ReturnsTrueForUnifiedPaths(string path)
    {
        Assert.IsTrue(CanonicalRoutePolicy.IsUnifiedRoute(path),
            $"Path '{path}' should be identified as a unified route.");
    }

    [TestMethod]
    [DataRow("/Home/Index")]
    [DataRow("/Admin")]
    [DataRow("")]
    [DataRow(null)]
    public void CanonicalRoutePolicy_IsUnifiedRoute_ReturnsFalseForNonUnifiedPaths(string? path)
    {
        Assert.IsFalse(CanonicalRoutePolicy.IsUnifiedRoute(path),
            $"Path '{path}' should NOT be identified as a unified route.");
    }
}
