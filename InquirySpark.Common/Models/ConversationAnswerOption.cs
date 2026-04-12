using System.Text.Json.Serialization;

namespace InquirySpark.Common.Models;

/// <summary>
/// A predefined answer option for a survey question.
/// </summary>
public class ConversationAnswerOption
{
    /// <summary>
    /// Gets or sets the answer option identifier (QuestionAnswerId).
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the display text for this answer option.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sort order for display.
    /// </summary>
    [JsonPropertyName("sort")]
    public int Sort { get; set; }
}
