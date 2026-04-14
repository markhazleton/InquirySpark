using Microsoft.AspNetCore.Html;

namespace InquirySpark.Web.Models.Unified;

/// <summary>
/// Model for the <c>_DataTableCard</c> partial view.
/// Drives the card chrome (header, table shell, footer) for all unified list views.
/// The table body content is injected via <see cref="TableBody"/>.
/// </summary>
public sealed class DataTableCardModel
{
    /// <summary>Gets or sets the card header title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the Bootstrap Icon class for the header (e.g., "bi-list").</summary>
    public string Icon { get; set; } = "bi-table";

    /// <summary>Gets or sets the capability identifier badge (e.g., "CAP-IA-006").</summary>
    public string? CapabilityId { get; set; }

    /// <summary>Gets or sets the href for the primary "Create New" button. Null hides the button.</summary>
    public string? CreateHref { get; set; }

    /// <summary>Gets or sets the label for the create button.</summary>
    public string CreateLabel { get; set; } = "Create New";

    /// <summary>Gets or sets whether the DataTables export (Excel/PDF/CSV) buttons are enabled.</summary>
    public bool ExportEnabled { get; set; }

    /// <summary>Gets or sets the HTML id attribute for the table element.</summary>
    public string TableId { get; set; } = "dataTable";

    /// <summary>Gets or sets the total record count shown in the card footer.</summary>
    public int? TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the rendered table content (thead + tbody) injected into the table element.
    /// Build using <c>new HtmlString(await renderHelper.RenderViewAsync(...))</c> or inline HTML.
    /// </summary>
    public IHtmlContent? TableBody { get; set; }

    /// <summary>Gets or sets the column header definitions rendered as &lt;th&gt; elements.</summary>
    public IReadOnlyList<DataTableColumn> Columns { get; set; } = [];
}

/// <summary>
/// Defines a single column header in a DataTable card.
/// </summary>
public sealed class DataTableColumn
{
    /// <summary>Gets or sets the column display label.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Gets or sets the Bootstrap Icon class for the column header (optional).</summary>
    public string? Icon { get; set; }

    /// <summary>Gets or sets whether this column has sorting disabled (adds class="no-sort").</summary>
    public bool NoSort { get; set; }
}
