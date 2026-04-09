namespace InquirySpark.Common.Models;

/// <summary>
/// HATEOAS response envelope for all Conversation API responses.
/// </summary>
public class ConversationEnvelope
{
    /// <summary>
    /// Gets or sets the unique conversation identifier.
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// Gets or sets the current action the client should render.
    /// </summary>
    public ConversationAction Action { get; set; } = new();

    /// <summary>
    /// Gets or sets the HATEOAS link for the next step. Null when conversation is complete.
    /// </summary>
    public string NextUrl { get; set; }

    /// <summary>
    /// Gets or sets the HATEOAS link for the previous step. Null at the first question.
    /// </summary>
    public string PrevUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the conversation has ended (all questions answered).
    /// </summary>
    public bool ConversationEnded { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp of the last update.
    /// </summary>
    public DateTime UpdatedUtc { get; set; }
}
