namespace InquirySpark.Web.Configuration.Unified;

/// <summary>
/// Configuration model for client UI framework settings in InquirySpark.Web.
/// Bound from appsettings.json → "ClientUi".
/// </summary>
public sealed class ClientUiOptions
{
    /// <summary>Gets or sets the Bootswatch theme name (e.g., "yeti", "flatly", "litera").</summary>
    public string BootswatchTheme { get; set; } = "yeti";

    /// <summary>Gets or sets whether DataTables state saving is enabled globally.</summary>
    public bool DataTablesStateSave { get; set; } = true;

    /// <summary>Gets or sets the DataTables page length default.</summary>
    public int DataTablesPageLength { get; set; } = 25;

    /// <summary>Gets or sets whether the export buttons extension is enabled on export-capable tables.</summary>
    public bool DataTablesExportEnabled { get; set; } = true;
}
