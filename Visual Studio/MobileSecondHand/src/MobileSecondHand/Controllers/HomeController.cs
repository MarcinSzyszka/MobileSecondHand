using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MobileSecondHand.Controllers {
	public class HomeController : Controller
    {
		private ILogger<HomeController> logger;

		public HomeController(ILoggerFactory loggerFactory)
		{
			this.logger = loggerFactory.CreateLogger<HomeController>();
		}
        public IActionResult Index()
        {
			logger.LogInformation("Hello world");
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
