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

        public async Task<ApiResponse<ChatGPTResponse?>> SendChatGPTRequest(ChatGPTRequest request)
        {
            try
            {
                string endpoint = _config["OpenAI:Endpoint"] ?? string.Empty;
                string key = _config["OpenAI:Key"] ?? string.Empty;

                _httpService.PostAsync<ChatGPTResponse?>("https://api.openai.com/v1/chat/completions", );

                var response = await client.PostAsync(endpoint, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    return await CreateErrorCodeResponse<ChatGPTResponse>(response);
                }

                var content = await response.Content.ReadAsStringAsync();
                var chatGPTResponse = JsonSerializer.Deserialize<ChatGPTResponse?>(content, GetJsonSerializerOptions());

                return new ApiResponse<ChatGPTResponse?>(chatGPTResponse);
            }
            catch (Exception ex)
            {
                return CreateFallbackResponse<ChatGPTResponse>(ex);
            }
        }
    }
}
