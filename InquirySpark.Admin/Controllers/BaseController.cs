using Microsoft.AspNetCore.Authorization;

namespace InquirySpark.Admin.Controllers;

/// <summary>
/// Provides a base controller with logging support for all admin controllers.
/// </summary>
[Authorize]
public abstract class BaseController : Controller
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for the controller.</param>
    protected BaseController(ILogger<BaseController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// The logger instance for the controller.
    /// </summary>
    protected readonly ILogger<BaseController> _logger;
}
