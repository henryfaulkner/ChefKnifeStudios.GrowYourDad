using ChefKnifeStudios.GrowYourDad.GoalDesigner.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using ChefKnife.HttpService.ApiResponse;

namespace ChefKnifeStudios.GrowYourDad.GoalDesigner.Controllers
{
    public class HomeController : Controller
    {
        readonly ILogger<HomeController> _logger;
        readonly IHttpService _httpService;
        readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger,
            IHttpService httpService,
            IConfiguration config)
        {
            _logger = logger;
            _httpService = httpService;
            _config = config;

            string key = _config["OpenAI:Key"] ?? string.Empty;
            _httpService.SetBearerAuthentication(key);
        }

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

        [HttpGet("generate-goal")]
        public async Task<IActionResult> GenerateGoalAsync()
        {
            string prompt = "Prewritten prompt";
            string endpoint = _config["OpenAI:Endpoint"] ?? string.Empty;
            var request = new ChatGPTRequest
            {
                Model = "gpt-4",
                Messages = new List<ChatGPTMessage>
                {
                    new ChatGPTMessage { Role = "system", Content = "You are a helpful assistant. I can put the robot in a certain frame of mind here." },
                    new ChatGPTMessage { Role = "user", Content = prompt }
                },
                Temperature = 0.7f
            };

            var res = await _httpService.PostAsync<ChatGPTRequest, ChatGPTResponse?>(endpoint, request);

            return Ok(res.Data);
        }
    }
}
