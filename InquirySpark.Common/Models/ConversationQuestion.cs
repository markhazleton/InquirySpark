using System.Text.Json.Serialization;

namespace InquirySpark.Common.Models;

/// <summary>
/// A single question presented to the user during a conversation step.
/// </summary>
public class ConversationQuestion
{
    /// <summary>
    /// Gets or sets the question identifier.
    /// </summary>
    [JsonPropertyName("question_id")]
    public int QuestionId { get; set; }

    /// <summary>
    /// Gets or sets the question group identifier.
    /// </summary>
    [JsonPropertyName("question_group_id")]
    public int QuestionGroupId { get; set; }

    /// <summary>
    /// Gets or sets the question text displayed to the user.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of predefined answer options.
    /// </summary>
    [JsonPropertyName("options")]
    public List<ConversationAnswerOption> Options { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the user may provide free-text input.
    /// </summary>
    [JsonPropertyName("allow_free_text")]
    public bool AllowFreeText { get; set; }
}
