namespace InquirySpark.Repository.Services;

/// <summary>
/// Represents a domain error in the Conversation API that maps to a specific HTTP status code.
/// Thrown by ConversationService to signal 400/401/404 error conditions.
/// </summary>
public class ConversationApiException : Exception
{
    /// <summary>
    /// Gets the HTTP status code to return to the caller.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ConversationApiException"/>.
    /// </summary>
    /// <param name="statusCode">The HTTP status code (e.g. 400, 401, 404).</param>
    /// <param name="message">The user-facing error message.</param>
    public ConversationApiException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
