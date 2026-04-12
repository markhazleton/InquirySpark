using InquirySpark.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace InquirySpark.Admin.Controllers.Api;

/// <summary>
/// Shared response mapper for API controllers returning BaseResponse wrappers.
/// </summary>
public static class ApiResponseHelper
{
    /// <summary>
    /// Converts a <see cref="BaseResponse{T}"/> to an <see cref="IActionResult"/>.
    /// </summary>
    public static IActionResult ToActionResult<T>(ControllerBase controller, BaseResponse<T> result)
    {
        if (result.IsSuccessful)
            return controller.Ok(result.Data);

        var firstError = result.Errors.FirstOrDefault() ?? "An error occurred.";
        var statusCode = ParseStatusCode(firstError, out var message);

        return statusCode switch
        {
            401 => controller.Unauthorized(new { error = message }),
            404 => controller.NotFound(new { error = message }),
            500 => controller.StatusCode(StatusCodes.Status500InternalServerError, new { error = message }),
            _ => controller.BadRequest(result.Errors)
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
