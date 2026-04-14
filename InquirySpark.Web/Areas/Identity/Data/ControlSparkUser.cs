// TODO(identity-migration): Namespace kept as ControlSpark.WebMvc.Areas.Identity.Data for EF Core identity schema
// compatibility — the ControlSparkUser.db SQLite schema was originally created under the ControlSpark.WebMvc
// namespace and the table/column names are locked to it. Rename to InquirySpark.Web.Areas.Identity.Data once
// the identity database schema is migrated. See spec FR-015 and contracts/decommission-verification-evidence.md.
using Microsoft.AspNetCore.Identity;

namespace ControlSpark.WebMvc.Areas.Identity.Data;

/// <summary>
/// Application user for InquirySpark.Web (canonical identity — shared with InquirySpark.Admin
/// over ControlSparkUserContextConnection). Maintains the same schema as InquirySpark.Admin.
/// </summary>
public class ControlSparkUser : IdentityUser
{
    /// <summary>Gets or sets the first name of the user.</summary>
    public string? FirstName { get; set; }

    /// <summary>Gets or sets the last name of the user.</summary>
    public string? LastName { get; set; }

    /// <summary>Gets or sets the profile picture bytes.</summary>
    public byte[]? ProfilePicture { get; set; }
}
