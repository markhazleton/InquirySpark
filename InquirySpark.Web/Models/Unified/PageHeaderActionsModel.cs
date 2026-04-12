namespace InquirySpark.Web.Models.Unified;

/// <summary>
/// Model for the <c>_PageHeaderActions</c> partial view.
/// Drives the page header with title, optional subtitle, capability badge, and action buttons.
/// </summary>
public sealed class PageHeaderActionsModel
{
    /// <summary>Gets or sets the page title displayed in the header.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets an optional subtitle displayed below the title.</summary>
    public string? Subtitle { get; set; }

    /// <summary>Gets or sets the Bootstrap Icon class (e.g., "bi-gear") for the header icon.</summary>
    public string Icon { get; set; } = "bi-grid";

    /// <summary>Gets or sets the capability identifier badge (e.g., "CAP-IA-001").</summary>
    public string? CapabilityId { get; set; }

    /// <summary>Gets or sets the primary action button (rendered as filled btn-primary).</summary>
    public PageHeaderAction? PrimaryAction { get; set; }

    /// <summary>Gets or sets secondary action buttons (rendered as btn-outline-*).</summary>
    public IReadOnlyList<PageHeaderAction> SecondaryActions { get; set; } = [];

    /// <summary>Gets or sets the URL for a back/cancel navigation link.</summary>
    public string? BackHref { get; set; }

    /// <summary>Gets or sets the label for the back navigation link.</summary>
    public string BackLabel { get; set; } = "Back";
}

/// <summary>
/// Represents an individual action button in the page header.
/// </summary>
public sealed class PageHeaderAction
{
    /// <summary>Gets or sets the button display label.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Gets or sets the URL the button links to.</summary>
    public string Href { get; set; } = "#";

    /// <summary>Gets or sets the Bootstrap Icon class (e.g., "bi-plus-circle").</summary>
    public string Icon { get; set; } = "bi-arrow-right";

    /// <summary>Gets or sets the tooltip title for the button.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the Bootstrap button color variant (e.g., "info", "warning").</summary>
    public string ButtonVariant { get; set; } = "secondary";
}
