#nullable enable
using InquirySpark.Common.Models.UnifiedWeb;
using InquirySpark.Repository.Services.UnifiedWeb;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InquirySpark.Common.Tests.UnifiedWeb;

/// <summary>
/// Unit/integration tests for <see cref="UnifiedWebCapabilityService"/> covering:
/// capability inventory, parity validation, cutover recording, rollback (US3 — T047B).
/// </summary>
[TestClass]
public class US3CapabilityServiceTests
{
    // ── Helpers ───────────────────────────────────────────────────────────

    private static UnifiedWebCapabilityService BuildService(List<CapabilityItem>? capabilities = null, List<CapabilityPhaseItem>? phases = null)
    {
        var options = Options.Create(new UnifiedWebOptions
        {
            CapabilityCompletion = new CapabilityCompletionOptions
            {
                Capabilities = capabilities ?? SampleCapabilities(),
                Phases = phases ?? SamplePhases(),
            },
        });
        return new UnifiedWebCapabilityService(options, NullLogger<UnifiedWebCapabilityService>.Instance);
    }

    private static List<CapabilityItem> SampleCapabilities() =>
    [
        new() { CapabilityId = "CAP-DS-001", Domain = "Decision Workspace", Name = "Conversations",   Phase = 2, Status = "deployed" },
        new() { CapabilityId = "CAP-DS-002", Domain = "Decision Workspace", Name = "Specifications",  Phase = 3, Status = "validated" },
        new() { CapabilityId = "CAP-IA-001", Domain = "Inquiry Administration", Name = "Applications", Phase = 2, Status = "deployed" },
        new() { CapabilityId = "CAP-IA-006", Domain = "Inquiry Authoring",       Name = "Surveys",     Phase = 1, Status = "scoped" },
    ];

    private static List<CapabilityPhaseItem> SamplePhases() =>
    [
        new() { PhaseNumber = 0, PhaseName = "Not Started", Description = "No work begun" },
        new() { PhaseNumber = 2, PhaseName = "Deployed",    Description = "Code in InquirySpark.Web" },
        new() { PhaseNumber = 3, PhaseName = "Validated",   Description = "Parity verified" },
        new() { PhaseNumber = 4, PhaseName = "Cut Over",    Description = "Legacy decommissioned" },
    ];

    // ── Capability inventory ──────────────────────────────────────────────

    [TestMethod]
    public async Task GetCapabilityInventoryAsync_ReturnsAllCapabilities()
    {
        var service = BuildService();
        var result = await service.GetCapabilityInventoryAsync();
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(4, result.Data?.Count);
    }

    [TestMethod]
    public async Task GetCapabilitiesByDomainAsync_ReturnsOnlyMatchingDomain()
    {
        var service = BuildService();
        var result = await service.GetCapabilitiesByDomainAsync("Decision Workspace");
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(2, result.Data?.Count);
        Assert.IsTrue(result.Data!.All(c => c.Domain == "Decision Workspace"));
    }

    [TestMethod]
    public async Task GetCapabilitiesByDomainAsync_CaseInsensitive_ReturnsResults()
    {
        var service = BuildService();
        var upper = await service.GetCapabilitiesByDomainAsync("DECISION WORKSPACE");
        var lower = await service.GetCapabilitiesByDomainAsync("decision workspace");
        Assert.AreEqual(upper.Data?.Count, lower.Data?.Count);
    }

    [TestMethod]
    public async Task GetCapabilitiesByDomainAsync_UnknownDomain_ReturnsEmptyCollection()
    {
        var service = BuildService();
        var result = await service.GetCapabilitiesByDomainAsync("Nonexistent Domain");
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(0, result.Data?.Count);
    }

    [TestMethod]
    public async Task GetCapabilityPhaseStatusAsync_KnownCapability_ReturnsItem()
    {
        var service = BuildService();
        var result = await service.GetCapabilityPhaseStatusAsync("CAP-DS-001");
        Assert.IsTrue(result.IsSuccessful);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual("CAP-DS-001", result.Data.CapabilityId);
    }

    [TestMethod]
    public async Task GetCapabilityPhaseStatusAsync_UnknownCapability_ReturnsFailure()
    {
        var service = BuildService();
        var result = await service.GetCapabilityPhaseStatusAsync("CAP-UNKNOWN-999");
        Assert.IsFalse(result.IsSuccessful);
    }

    // ── Parity validation ─────────────────────────────────────────────────

    [TestMethod]
    public async Task SubmitParityValidationAsync_FullyValidatedRecord_AdvancesCapabilityToPhase3()
    {
        var caps = SampleCapabilities();
        // CAP-DS-001 starts at Phase 2
        var service = BuildService(caps);

        var record = new ParityValidationRecordItem
        {
            CapabilityId = "CAP-DS-001",
            ValidatedBy = "tester",
            FunctionalParityPassed = true,
            PermissionParityPassed = true,
            UxConsistencyPassed = true,
            PerformancePassed = true,
        };

        var result = await service.SubmitParityValidationAsync(record);

        Assert.IsTrue(result.IsSuccessful);
        // Capability should have advanced to Phase 3
        var capStatus = await service.GetCapabilityPhaseStatusAsync("CAP-DS-001");
        Assert.AreEqual(3, capStatus.Data?.Phase);
    }

    [TestMethod]
    public async Task SubmitParityValidationAsync_PartialRecord_DoesNotAdvancePhase()
    {
        var caps = SampleCapabilities();
        var service = BuildService(caps);

        var record = new ParityValidationRecordItem
        {
            CapabilityId = "CAP-DS-001",
            ValidatedBy = "tester",
            FunctionalParityPassed = true,
            PermissionParityPassed = false, // not fully validated
            UxConsistencyPassed = true,
            PerformancePassed = true,
        };

        await service.SubmitParityValidationAsync(record);

        var capStatus = await service.GetCapabilityPhaseStatusAsync("CAP-DS-001");
        Assert.AreEqual(2, capStatus.Data?.Phase, "Partial validation must not advance the phase.");
    }

    // ── Cutover decision ──────────────────────────────────────────────────

    [TestMethod]
    public async Task RecordCutoverDecisionAsync_GoDecision_AdvancesDomainCapabilitiesToPhase4()
    {
        var service = BuildService();

        // First validate CAP-DS-001 and CAP-DS-002 to Phase 3
        await service.SubmitParityValidationAsync(new ParityValidationRecordItem
        {
            CapabilityId = "CAP-DS-001",
            ValidatedBy = "tester",
            FunctionalParityPassed = true,
            PermissionParityPassed = true,
            UxConsistencyPassed = true,
            PerformancePassed = true,
        });

        var decision = new CutoverDecisionRecordItem
        {
            Domain = "Decision Workspace",
            LegacyApp = "DecisionSpark",
            Decision = "Go",
            ApprovedBy = "approver",
            IsCutOver = true,
            DecidedAt = DateTimeOffset.UtcNow,
        };

        var result = await service.RecordCutoverDecisionAsync(decision);

        Assert.IsTrue(result.IsSuccessful);

        var ds001 = await service.GetCapabilityPhaseStatusAsync("CAP-DS-001");
        var ds002 = await service.GetCapabilityPhaseStatusAsync("CAP-DS-002");

        Assert.AreEqual(4, ds001.Data?.Phase, "CAP-DS-001 should be Phase 4 after Go decision.");
        Assert.AreEqual(4, ds002.Data?.Phase, "CAP-DS-002 should be Phase 4 after Go decision.");
    }

    [TestMethod]
    public async Task RecordCutoverDecisionAsync_NoGoDecision_DoesNotAdvancePhase()
    {
        var service = BuildService();
        var decision = new CutoverDecisionRecordItem
        {
            Domain = "Decision Workspace",
            LegacyApp = "DecisionSpark",
            Decision = "No-Go",
            ApprovedBy = "approver",
            IsCutOver = false,
            DecidedAt = DateTimeOffset.UtcNow,
        };

        var result = await service.RecordCutoverDecisionAsync(decision);

        Assert.IsTrue(result.IsSuccessful);
        var ds001 = await service.GetCapabilityPhaseStatusAsync("CAP-DS-001");
        Assert.AreEqual(2, ds001.Data?.Phase, "No-Go decision must not change phase.");
    }

    // ── Rollback ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task RevertDomainCutoverAsync_AfterGoDecision_RevertsCapabilitiesToPhase3()
    {
        var service = BuildService();

        // Advance to Phase 4 via Go decision
        await service.RecordCutoverDecisionAsync(new CutoverDecisionRecordItem
        {
            Domain = "Decision Workspace",
            LegacyApp = "DecisionSpark",
            Decision = "Go",
            ApprovedBy = "approver",
            IsCutOver = true,
            DecidedAt = DateTimeOffset.UtcNow,
        });

        // Rollback
        var rollback = await service.RevertDomainCutoverAsync("Decision Workspace", "ops-lead", "Test rollback scenario");

        Assert.IsTrue(rollback.IsSuccessful);
        Assert.IsTrue(rollback.Data);

        var ds002 = await service.GetCapabilityPhaseStatusAsync("CAP-DS-002");
        Assert.AreEqual(3, ds002.Data?.Phase, "Capabilities should revert to Phase 3 after rollback.");
    }

    [TestMethod]
    public async Task RevertDomainCutoverAsync_EmptyDomain_ThrowsArgumentException()
    {
        var service = BuildService();
        try
        {
            await service.RevertDomainCutoverAsync("", "ops-lead", "reason");
            Assert.Fail("Expected ArgumentException was not thrown.");
        }
        catch (ArgumentException)
        {
            // expected
        }
    }

    // ── Phase listing ──────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetCompletionPhasesAsync_ReturnsAllConfiguredPhases()
    {
        var service = BuildService();
        var result = await service.GetCompletionPhasesAsync();
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(4, result.Data?.Count);
    }
}
