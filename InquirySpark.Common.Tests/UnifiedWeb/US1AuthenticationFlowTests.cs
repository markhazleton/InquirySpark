#nullable enable
using InquirySpark.Common.Models.UnifiedWeb;
using InquirySpark.Repository.Services.UnifiedWeb;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InquirySpark.Common.Tests.UnifiedWeb;

/// <summary>
/// Integration tests for authentication and session continuity in InquirySpark.Web (US1 / T019A).
/// Validates that the reused InquirySpark.Admin sign-in experience produces correct
/// unified-role mappings and that the canonical identity bridge honours FR-004 / FR-015.
/// </summary>
[TestClass]
public class US1AuthenticationFlowTests
{
    // ── Helpers ───────────────────────────────────────────────────────────────

    private static IdentityMigrationBridgeService BuildService(List<RoleMappingItem> mappings)
    {
        var opts = Options.Create(new IdentityBridgeOptions { RoleMappings = mappings });
        return new IdentityMigrationBridgeService(opts, NullLogger<IdentityMigrationBridgeService>.Instance);
    }

    private static List<RoleMappingItem> AdminRoleMappings() =>
    [
        new() { SourceApp = "InquirySpark.Admin", SourceRole = "Administrator", UnifiedRole = "Administrator", IsEquivalent = true },
        new() { SourceApp = "InquirySpark.Admin", SourceRole = "Analyst",       UnifiedRole = "Analyst",       IsEquivalent = true },
        new() { SourceApp = "InquirySpark.Admin", SourceRole = "Operator",      UnifiedRole = "Operator",      IsEquivalent = true },
        new() { SourceApp = "InquirySpark.Admin", SourceRole = "Consultant",    UnifiedRole = "Consultant",    IsEquivalent = true },
        new() { SourceApp = "InquirySpark.Admin", SourceRole = "Executive",     UnifiedRole = "Executive",     IsEquivalent = true },
    ];

    // ── Role mapping — happy paths ────────────────────────────────────────────

    [TestMethod]
    [DataRow("Administrator")]
    [DataRow("Analyst")]
    [DataRow("Operator")]
    [DataRow("Consultant")]
    [DataRow("Executive")]
    public async Task GetUnifiedRoles_KnownAdminRole_ReturnsMappedRole(string sourceRole)
    {
        var service = BuildService(AdminRoleMappings());

        var result = await service.GetUnifiedRolesForLegacyRoleAsync("InquirySpark.Admin", sourceRole);

        Assert.IsTrue(result.IsSuccessful, "Expected successful response for known role.");
        Assert.AreEqual(1, result.Data?.Count, $"Expected exactly one mapped role for '{sourceRole}'.");
        Assert.AreEqual(sourceRole, result.Data![0], StringComparer.OrdinalIgnoreCase,
            "Unified role name should equal source role for direct-equivalent mappings.");
    }

    [TestMethod]
    public async Task GetUnifiedRoles_UnknownRole_ReturnsEmptyList()
    {
        var service = BuildService(AdminRoleMappings());

        var result = await service.GetUnifiedRolesForLegacyRoleAsync("InquirySpark.Admin", "UnknownRole");

        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(0, result.Data?.Count, "Unknown role should yield no mappings.");
    }

    [TestMethod]
    public async Task GetUnifiedRoles_UnknownApp_ReturnsEmptyList()
    {
        var service = BuildService(AdminRoleMappings());

        var result = await service.GetUnifiedRolesForLegacyRoleAsync("SomeOtherApp", "Administrator");

        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(0, result.Data?.Count, "Roles from unregistered apps should not be mapped.");
    }

    // ── Role mapping — app-level enumeration ──────────────────────────────────

    [TestMethod]
    public async Task GetRoleMappingsForApp_AdminApp_ReturnsAllFiveRoles()
    {
        var service = BuildService(AdminRoleMappings());

        var result = await service.GetRoleMappingsForAppAsync("InquirySpark.Admin");

        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(5, result.Data?.Count, "All five InquirySpark.Admin roles must have mappings.");
        Assert.IsTrue(result.Data!.All(m => m.IsEquivalent),
            "All InquirySpark.Admin roles should be flagged as equivalent in the canonical mapping.");
    }

    [TestMethod]
    public async Task GetRoleMappingsForApp_CaseInsensitiveAppName()
    {
        var service = BuildService(AdminRoleMappings());

        var lower = await service.GetRoleMappingsForAppAsync("inquiryspark.admin");
        var upper = await service.GetRoleMappingsForAppAsync("INQUIRYSPARK.ADMIN");

        Assert.AreEqual(5, lower.Data?.Count, "Case-insensitive lookup (lowercase) must return all 5 roles.");
        Assert.AreEqual(5, upper.Data?.Count, "Case-insensitive lookup (uppercase) must return all 5 roles.");
    }

    // ── Parity verification ──────────────────────────────────────────────────

    [TestMethod]
    public async Task VerifyRoleParity_EquivalentMapping_ReturnsTrue()
    {
        var service = BuildService(AdminRoleMappings());

        var result = await service.VerifyRoleParityAsync("user-123", "InquirySpark.Admin", "Analyst");

        Assert.IsTrue(result.IsSuccessful);
        Assert.IsTrue(result.Data, "Equivalent role mappings should pass parity verification.");
    }

    [TestMethod]
    public async Task VerifyRoleParity_NoMapping_ReturnsFailure()
    {
        var service = BuildService(AdminRoleMappings());

        var result = await service.VerifyRoleParityAsync("user-123", "InquirySpark.Admin", "Ghost");

        Assert.IsFalse(result.IsSuccessful, "Missing mapping should yield a failed parity check.");
        Assert.IsTrue(result.Errors.Count > 0, "Failure should includes an error message.");
    }

    [TestMethod]
    public async Task VerifyRoleParity_NonEquivalentMapping_SucceedsButFlagsFalse()
    {
        var nonEquivalentMappings = new List<RoleMappingItem>
        {
            new() { SourceApp = "InquirySpark.Admin", SourceRole = "CustomRole", UnifiedRole = "LimitedRole", IsEquivalent = false }
        };
        var service = BuildService(nonEquivalentMappings);

        var result = await service.VerifyRoleParityAsync("user-999", "InquirySpark.Admin", "CustomRole");

        Assert.IsTrue(result.IsSuccessful, "A non-equivalent mapping should still return a successful response.");
        Assert.IsFalse(result.Data, "Non-equivalent mapping should return false (requires manual review).");
    }

    // ── Privilege escalation detection ───────────────────────────────────────

    [TestMethod]
    public async Task GetPrivilegeEscalationRisks_AllEquivalentMappings_ReturnsEmpty()
    {
        var service = BuildService(AdminRoleMappings());

        var result = await service.GetPrivilegeEscalationRisksAsync();

        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(0, result.Data?.Count,
            "No escalation risks expected when all mappings are equivalent.");
    }

    [TestMethod]
    public async Task GetPrivilegeEscalationRisks_NonEquivalentMapping_IsReturned()
    {
        var mixedMappings = AdminRoleMappings().ToList();
        mixedMappings.Add(new RoleMappingItem
        {
            SourceApp = "InquirySpark.Admin",
            SourceRole = "PowerUser",
            UnifiedRole = "Administrator",
            IsEquivalent = false
        });
        var service = BuildService(mixedMappings);

        var result = await service.GetPrivilegeEscalationRisksAsync();

        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(1, result.Data?.Count, "Non-equivalent mapping should appear as escalation risk.");
        Assert.AreEqual("PowerUser", result.Data![0].SourceRole);
    }

    // ── Session continuity — canonical identity authority semantics ───────────

    [TestMethod]
    public void IdentityBridgeOptions_DefaultRoleMappings_IsNullSafe()
    {
        // Constructing options with null RoleMappings must not blow up the service.
        var opts = Options.Create(new IdentityBridgeOptions { RoleMappings = null });
        var service = new IdentityMigrationBridgeService(opts, NullLogger<IdentityMigrationBridgeService>.Instance);

        // Should not throw
        Assert.IsNotNull(service);
    }

    [TestMethod]
    public async Task GetRoleMappingsForApp_NullMappingsList_ReturnsEmptyCollection()
    {
        var opts = Options.Create(new IdentityBridgeOptions { RoleMappings = null });
        var service = new IdentityMigrationBridgeService(opts, NullLogger<IdentityMigrationBridgeService>.Instance);

        var result = await service.GetRoleMappingsForAppAsync("InquirySpark.Admin");

        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual(0, result.Data?.Count,
            "Null RoleMappings list should gracefully return an empty collection.");
    }

    [TestMethod]
    public async Task GetUnifiedRoles_ArgumentNullOrEmpty_ThrowsArgumentException()
    {
        var service = BuildService(AdminRoleMappings());

        // sourceApp empty
        try
        {
            await service.GetUnifiedRolesForLegacyRoleAsync(string.Empty, "Administrator");
            Assert.Fail("Expected ArgumentException for empty sourceApp.");
        }
        catch (ArgumentException)
        {
            // expected
        }

        // sourceRole empty
        try
        {
            await service.GetUnifiedRolesForLegacyRoleAsync("InquirySpark.Admin", string.Empty);
            Assert.Fail("Expected ArgumentException for empty sourceRole.");
        }
        catch (ArgumentException)
        {
            // expected
        }
    }
}
