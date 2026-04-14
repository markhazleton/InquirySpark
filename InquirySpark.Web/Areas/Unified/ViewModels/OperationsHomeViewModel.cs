namespace InquirySpark.Web.Areas.Unified.ViewModels;

/// <summary>
/// View model for the unified operations home dashboard.
/// Aggregates capability-family surface area counts for the landing page.
/// </summary>
public sealed class OperationsHomeViewModel
{
    /// <summary>Gets or sets the greeting name for the signed-in user.</summary>
    public string UserDisplayName { get; set; } = string.Empty;

    /// <summary>Gets or sets the total number of registered capabilities.</summary>
    public int TotalCapabilities { get; set; }

    /// <summary>Gets or sets the number of capabilities with status 'not-started'.</summary>
    public int CapabilitiesNotStarted { get; set; }

    /// <summary>Gets or sets the number of capabilities in progress.</summary>
    public int CapabilitiesInProgress { get; set; }

    /// <summary>Gets or sets the number of validated/completed capabilities.</summary>
    public int CapabilitiesValidated { get; set; }

    /// <summary>Gets or sets the number of capability domains in the inventory.</summary>
    public int DomainCount { get; set; }

    /// <summary>Gets the completion percentage (0–100).</summary>
    public int CompletionPercent =>
        TotalCapabilities == 0 ? 0 :
        (int)Math.Round((CapabilitiesValidated * 100.0) / TotalCapabilities);
}
