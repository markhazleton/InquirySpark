namespace InquirySpark.Web.Areas.Unified.ViewModels.Completion;

/// <summary>
/// View model for the capability completion matrix dashboard (CAP-IA — Capability Matrix).
/// </summary>
public sealed class CapabilityCompletionMatrixViewModel
{
    /// <summary>Gets or sets capability items grouped by domain.</summary>
    public IReadOnlyList<CapabilityDomainGroup> DomainGroups { get; set; } = [];

    /// <summary>Gets or sets the overall capability counts.</summary>
    public CapabilityMatrixSummary Summary { get; set; } = new();

    /// <summary>Gets or sets the available completion phases.</summary>
    public IReadOnlyList<PhaseInfo> Phases { get; set; } = [];
}

/// <summary>Capability items grouped under a single domain family.</summary>
public sealed class CapabilityDomainGroup
{
    /// <summary>Gets or sets the domain name (e.g., "Decision Workspace").</summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>Gets or sets the Bootstrap icon class for this domain.</summary>
    public string Icon { get; set; } = "bi-grid";

    /// <summary>Gets or sets Bootstrap color variant for the domain header.</summary>
    public string ColorVariant { get; set; } = "primary";

    /// <summary>Gets or sets all capabilities in this domain.</summary>
    public IReadOnlyList<CapabilityMatrixRow> Capabilities { get; set; } = [];

    /// <summary>Gets the count of fully validated capabilities in this domain.</summary>
    public int ValidatedCount => Capabilities.Count(c => c.Phase >= 3);

    /// <summary>Gets the completion percentage for this domain.</summary>
    public int CompletionPercent =>
        Capabilities.Count == 0 ? 0 : ValidatedCount * 100 / Capabilities.Count;
}

/// <summary>A single capability row in the completion matrix.</summary>
public sealed class CapabilityMatrixRow
{
    /// <summary>Gets or sets the capability identifier (e.g., "CAP-DS-001").</summary>
    public string CapabilityId { get; set; } = string.Empty;

    /// <summary>Gets or sets the capability name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the current completion phase (0=Not Started, 1=Scoped, 2=Deployed, 3=Validated, 4=CutOver).</summary>
    public int Phase { get; set; }

    /// <summary>Gets or sets the current status label.</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Gets or sets the unified route for this capability.</summary>
    public string? UnifiedRoute { get; set; }

    /// <summary>Gets or sets optional notes about this capability's status.</summary>
    public string? Notes { get; set; }

    /// <summary>Gets the Bootstrap badge class for the current phase.</summary>
    public string PhaseBadgeClass => Phase switch
    {
        0 => "bg-secondary",
        1 => "bg-info",
        2 => "bg-warning text-dark",
        3 => "bg-success",
        4 => "bg-primary",
        _ => "bg-secondary",
    };

    /// <summary>Gets the human-readable phase label.</summary>
    public string PhaseLabel => Phase switch
    {
        0 => "Not Started",
        1 => "Scoped",
        2 => "Deployed",
        3 => "Validated",
        4 => "Cut Over",
        _ => "Unknown",
    };
}

/// <summary>Aggregate counts across all capabilities.</summary>
public sealed class CapabilityMatrixSummary
{
    /// <summary>Gets or sets the total capability count.</summary>
    public int Total { get; set; }

    /// <summary>Gets or sets the not-started count.</summary>
    public int NotStarted { get; set; }

    /// <summary>Gets or sets the deployed count.</summary>
    public int Deployed { get; set; }

    /// <summary>Gets or sets the validated count.</summary>
    public int Validated { get; set; }

    /// <summary>Gets or sets the cut-over count.</summary>
    public int CutOver { get; set; }

    /// <summary>Gets the overall completion percentage (validated + cut-over).</summary>
    public int CompletionPercent =>
        Total == 0 ? 0 : (Validated + CutOver) * 100 / Total;
}

/// <summary>A phase definition for display in the phase legend.</summary>
public sealed class PhaseInfo
{
    /// <summary>Gets or sets the numeric phase identifier.</summary>
    public int PhaseNumber { get; set; }

    /// <summary>Gets or sets the phase name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the phase description.</summary>
    public string Description { get; set; } = string.Empty;
}
