using InquirySpark.Common.Models;

namespace InquirySpark.Repository.Services;

/// <summary>
/// Service for managing HATEOAS-driven survey conversations.
/// </summary>
public interface IConversationService
{
    /// <summary>
    /// Authenticates the user and starts, resumes, or restarts a conversation.
    /// Returns a survey list when no survey_id is provided.
    /// </summary>
    /// <param name="request">The start request containing credentials and optional conversation context.</param>
    /// <returns>A conversation envelope with the first (or current) action.</returns>
    Task<BaseResponse<ConversationEnvelope>> StartConversationAsync(ConversationStartRequest request);

    /// <summary>
    /// Submits an answer for the current question and returns the next step.
    /// If no answer body is provided (null request), returns the current question without modification (read mode).
    /// </summary>
    /// <param name="conversationId">The unique conversation identifier.</param>
    /// <param name="questionId">The question being answered.</param>
    /// <param name="request">The answer payload. Null for read-mode access.</param>
    /// <returns>A conversation envelope with the next action or completion status.</returns>
    Task<BaseResponse<ConversationEnvelope>> NextStepAsync(
        Guid conversationId,
        int questionId,
        ConversationNextRequest request);
}
