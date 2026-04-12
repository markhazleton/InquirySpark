using System.Text.Json.Serialization;

namespace InquirySpark.Common.Models;

/// <summary>
/// Request body for POST /api/v1/conversation/next/{conversationId}/{questionId}.
/// When null or empty, returns the current question without modifying state (read mode).
/// </summary>
public class ConversationNextRequest
{
    /// <summary>
    /// Gets or sets the selected answer option identifier. Used for multiple-choice questions.
    /// </summary>
    [JsonPropertyName("question_answer_id")]
    public int? QuestionAnswerId { get; set; }

    /// <summary>
    /// Gets or sets the free-text answer. Used for open-ended questions.
    /// </summary>
    [JsonPropertyName("user_input")]
    public string UserInput { get; set; }
}
