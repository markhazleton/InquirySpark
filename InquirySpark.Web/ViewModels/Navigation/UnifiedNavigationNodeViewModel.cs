namespace InquirySpark.Web.ViewModels.Navigation;

/// <summary>
/// Represents a single navigation node in the unified InquirySpark.Web navigation tree.
/// Used by <see cref="Services.Navigation.UnifiedNavigationBuilder"/> to build the menu structure.
/// </summary>
public sealed class UnifiedNavigationNodeViewModel
{
    /// <summary>Gets or sets the display label for this navigation node.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Gets or sets the URL or relative path for this node. Empty for group headers.</summary>
    public string Href { get; set; } = string.Empty;

    /// <summary>Gets or sets the Bootstrap Icon class (e.g., "bi-house-door") for this node.</summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>Gets or sets the data-unified-nav attribute value used for active-link detection.</summary>
    public string NavKey { get; set; } = string.Empty;

    /// <summary>Gets or sets whether this node is a group/section header with children.</summary>
    public bool IsGroup { get; set; }

    /// <summary>Gets or sets child navigation nodes for dropdown/group nodes.</summary>
    public IReadOnlyList<UnifiedNavigationNodeViewModel> Children { get; set; } = [];

    /// <summary>Gets or sets an optional role policy required to display this node.</summary>
    public string? RequiredPolicy { get; set; }

    /// <summary>Gets or sets the display order for sorting sibling nodes.</summary>
    public int Order { get; set; }
}