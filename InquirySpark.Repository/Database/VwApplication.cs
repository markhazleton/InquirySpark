namespace InquirySpark.Repository.Database;

/// <summary>
/// Represents the VwApplication entity.
/// </summary>
public partial class VwApplication
{
    /// <summary>
    /// Gets or sets the application ID.
    /// </summary>
    [DisplayName("Application ID")]
    public int ApplicationId { get; set; }

    /// <summary>
    /// Gets or sets the application name.
    /// </summary>
    [DisplayName("Application Name")]
    public string ApplicationNm { get; set; } = null!;

    /// <summary>
    /// Gets or sets the application code.
    /// </summary>
    [DisplayName("Application Code")]
    public string ApplicationCd { get; set; } = null!;

    /// <summary>
    /// Gets or sets the short name of the application.
    /// </summary>
    [DisplayName("Application Short Name")]
    public string ApplicationShortNm { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description of the application.
    /// </summary>
    [DisplayName("Application Description")]
    public string? ApplicationDs { get; set; }

    /// <summary>
    /// Gets or sets the menu order of the application.
    /// </summary>
    [DisplayName("Menu Order")]
    public int MenuOrder { get; set; }

    /// <summary>
    /// Gets or sets the application type ID.
    /// </summary>
    [DisplayName("Application Type ID")]
    public int? ApplicationTypeId { get; set; }

    /// <summary>
    /// Gets or sets the name of the application type.
    /// </summary>
    [DisplayName("Application Type Name")]
    public string? ApplicationTypeNm { get; set; }

    /// <summary>
    /// Gets or sets the description of the application type.
    /// </summary>
    [DisplayName("Application Type Description")]
    public string? ApplicationTypeDs { get; set; }

    /// <summary>
    /// Gets or sets the count of surveys for the application.
    /// </summary>
    [DisplayName("Survey Count")]
    public int? SurveyCount { get; set; }

    /// <summary>
    /// Gets or sets the count of survey responses for the application.
    /// </summary>
    [DisplayName("Survey Response Count")]
    public int? SurveyResponseCount { get; set; }

    /// <summary>
    /// Gets or sets the count of users for the application.
    /// </summary>
    [DisplayName("User Count")]
    public int? UserCount { get; set; }
}
