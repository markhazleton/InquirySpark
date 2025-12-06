using System.Diagnostics;
using InquirySpark.Admin.Models;

namespace InquirySpark.Admin.Controllers
{
    public class HomeController(ILogger<BaseController> logger) : BaseController(logger)
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
