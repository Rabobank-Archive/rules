using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VstsService;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public IVstsScanner Scanner { get; }

        public HomeController(IVstsScanner scanner)
        {
            Scanner = scanner;
        }

        public IActionResult Index()
        {
            var model = Scanner.ScanProject("SOx-compliant-demo");
            return View(model);
        }

        [HttpGet]
        public IActionResult Scan(string projectName)
        {
            var model = Scanner.ScanProject(projectName);
            return View("ProjectRapport",model);
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