using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using YouTubeApiProject.Models;
using YouTubeApiProject.Services;

namespace YouTubeApiProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly YouTubeApiService _youtubeService;

        public HomeController(ILogger<HomeController> logger, YouTubeApiService youtubeService)
        {
            _logger = logger;
            _youtubeService = youtubeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string query, string duration, string uploadDate)
        {
            if (string.IsNullOrEmpty(query))
            {
                ViewBag.Message = "Please enter a search term.";
                return View("Index");
            }

            List<YouTubeVideoModel> results = await _youtubeService.SearchVideosAsync(query, duration, uploadDate);
            return View("Results", results);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
