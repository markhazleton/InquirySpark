using InquirySpark.Common.Models;
using InquirySpark.Repository.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InquirySpark.Admin.Controllers.Api;

/// <summary>
/// HATEOAS-driven Conversation API — walks a user through a survey one question at a time.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/v1/conversation")]
public class ConversationController(
    IConversationService service,
    ILogger<ConversationController> logger) : ControllerBase
{
    private readonly IConversationService _service = service;
    private readonly ILogger<ConversationController> _logger = logger;

    /// <summary>
    /// Authenticate and start, resume, or restart a survey conversation.
    /// Returns a survey list when no survey_id is provided.
    /// </summary>
    /// <param name="request">Credentials, application context, and optional survey/conversation identifiers.</param>
    /// <returns>A conversation envelope with the first question or survey selection list.</returns>
    [HttpPost("start")]
    [SwaggerOperation(
        Summary = "Start, resume, or restart a conversation",
        Description = "Authenticate the user and start a new survey, resume the last unanswered question, or restart all answers.")]
    [ProducesResponseType(typeof(ConversationEnvelope), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Start([FromBody] ConversationStartRequest request)
    {
        _logger.LogInformation("Conversation/Start called for application {ApplicationId}", request?.ApplicationId);
        var result = await _service.StartConversationAsync(request!);
        return MapResult(result);
    }

    /// <summary>
    /// Submit an answer and advance to the next question.
    /// </summary>
    /// <param name="conversationId">The unique conversation identifier.</param>
    /// <param name="questionId">The question being answered.</param>
    /// <param name="request">The answer payload. Omit body for read-mode (returns the current question without modification).</param>
    /// <returns>A conversation envelope with the next question or completion status.</returns>
    [HttpPost("next/{conversationId}/{questionId}")]
    [SwaggerOperation(
        Summary = "Submit answer and advance to next question",
        Description = "Submits an answer for the specified question and returns the next step envelope. Send an empty body to re-fetch the current question without saving an answer.")]
    [ProducesResponseType(typeof(ConversationEnvelope), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Next(
        [FromRoute] Guid conversationId,
        [FromRoute] int questionId,
        [FromBody] ConversationNextRequest? request)
    {
        _logger.LogInformation("Conversation/Next called for conversation {ConversationId} question {QuestionId}",
            conversationId, questionId);
        var result = await _service.NextStepAsync(conversationId, questionId, request!);
        return MapResult(result);
    }

    // ─── Private ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Maps a <see cref="BaseResponse{T}"/> to the appropriate <see cref="IActionResult"/>.
    /// Parses prefixed error messages (e.g. "401: Invalid credentials.") to determine HTTP status code.
    /// </summary>
    private IActionResult MapResult<T>(BaseResponse<T> result)
    {
        if (result.IsSuccessful)
            return Ok(result.Data);

        // Parse the first error for a status code prefix (e.g. "401: message")
        var firstError = result.Errors.FirstOrDefault() ?? "An error occurred.";
        var statusCode = ParseStatusCode(firstError, out var message);

        return statusCode switch
        {
            401 => Unauthorized(new { error = message }),
            404 => NotFound(new { error = message }),
            _ => BadRequest(result.Errors)
        };
    }

    private static int ParseStatusCode(string error, out string message)
    {
        if (error.Length > 4
            && int.TryParse(error[..3], out var code)
            && error[3] == ':')
        {
            message = error[5..].Trim();
            return code;
        }

        message = error;
        return 400;
    }
}
