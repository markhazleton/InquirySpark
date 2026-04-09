namespace InquirySpark.Common.Models;

/// <summary>
/// Request body for POST /api/v1/conversation/start.
/// </summary>
public class ConversationStartRequest
{
    /// <summary>
    /// Gets or sets the account name (username) for authentication.
    /// </summary>
    public string AccountName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the plaintext password for authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the application identifier that scopes the available surveys.
    /// </summary>
    public int ApplicationId { get; set; }

    /// <summary>
    /// Gets or sets the survey identifier to start. When null, the API returns a list of available surveys.
    /// </summary>
    public int? SurveyId { get; set; }

    /// <summary>
    /// Gets or sets the existing conversation identifier for resume or restart operations.
    /// </summary>
    public Guid? ConversationId { get; set; }

    /// <summary>
    /// Gets or sets the action to perform when ConversationId is provided.
    /// Valid values: "resume" (default) | "restart"
    /// </summary>
    public string Action { get; set; }
}
