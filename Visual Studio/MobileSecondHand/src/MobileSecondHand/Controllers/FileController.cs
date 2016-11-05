using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MobileSecondHand.API.Models.Config;

namespace MobileSecondHand.Controllers
{
	[Produces("application/json")]
	[Route("api/File")]
	public class FileController : Controller
	{
		private ILogger logger;
		private AppConfiguration appConfig;

		public FileController(ILoggerFactory loggerFactory, AppConfiguration appConfig)
		{
			logger = loggerFactory.CreateLogger<FileController>();
			this.appConfig = appConfig;
		}

		[HttpGet]
		[Route("reg")]
		public IActionResult GetReg()
		{
			var regPath = Path.Combine(appConfig.FileRepositoryPath, "Documents", "Regulamin.pdf");

			return File(new FileStream(regPath, FileMode.Open, FileAccess.Read, FileShare.Read), "application/pdf", "Regulamin.pdf");
		}

		[HttpGet]
		[Route("privpolicy")]
		public IActionResult GetPrivPolicy()
		{
			var privPolicyPath = Path.Combine(appConfig.FileRepositoryPath, "Documents", "Polityka prywatnosci.pdf");

			return File(new FileStream(privPolicyPath, FileMode.Open, FileAccess.Read, FileShare.Read), "application/pdf", "Polityka prywatnosci.pdf");
		}
	}
}