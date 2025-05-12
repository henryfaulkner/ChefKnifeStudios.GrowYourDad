using ChefKnifeStudios.GrowYourDad.GoalDesigner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace ChefKnifeStudios.GrowYourDad.GoalDesigner.Controllers
{
    public class HomeController : Controller
    {
        readonly ILogger<HomeController> _logger;
        readonly IHttpService _httpService;
        readonly IConfiguration _config;
        readonly IWebHostEnvironment _env;

        string _goalsJsonPath;

        public HomeController(ILogger<HomeController> logger,
            IHttpService httpService,
            IConfiguration config,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _httpService = httpService;
            _config = config;
            _env = env;

            string key = _config["OpenAI:Key"] ?? string.Empty;
            _httpService.SetBearerAuthentication(key);
            _goalsJsonPath = Path.Combine(_env.ContentRootPath, "App_Data", "goals.json");
        }

        #region CREATION
        public IActionResult Index()
        {
            var goals = GetStoredGoals();
            var goalsJson = JsonSerializer.Serialize(goals);

            var model = new GoalCreationModel()
            {
                Message = "placeholder text",
                Goals = goals ?? [],
                GoalsJson = goalsJson,
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult CreateGoal(GoalCreationModel model)
        {
            model.DeserializeGoals();

            var goals = model.Goals.ToList();

            int messageRating;
            if (goals.Any())
            {
                messageRating = goals.Select(x => x.Rating).Max() + 1;
            }
            else
            {
                messageRating = 1;
            }
            
            goals.Add(new Goal()
            {
                Message = model.Message,
                Rating = messageRating,
            });

            model.Message = string.Empty;
            model.Goals = goals;

            string updatedJson = JsonSerializer.Serialize(model.Goals, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_goalsJsonPath, updatedJson);

            ModelState.SetModelValue("Message", new ValueProviderResult(model.Message));
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> GetGptMessage(GoalCreationModel model)
        {
            model.DeserializeGoals();

            string prompt = "Prewritten prompt";
            string endpoint = _config["OpenAI:Endpoint"] ?? string.Empty;
            var request = new ChatGPTRequest
            {
                Model = "gpt-3.5-turbo",
                Messages = new List<ChatGPTMessage>
                {
                    new ChatGPTMessage { Role = "system", Content = "You are a machine with a single purpose. To create fictional accomplishment, which my dad has succeeded in achieving, in under 2 sentences. These accomplishment can be mental, social, career-oriented, or physical. All accomplishments should be explicitly determine with how much more buff my dad has become recently." },
                    new ChatGPTMessage { Role = "user", Content = prompt }
                },
                Temperature = 0.7f
            };

            var res = await _httpService.PostAsync<ChatGPTRequest, ChatGPTResponse?>(endpoint, request);

            model.Message = res.Data?.Choices.FirstOrDefault()?.Message.Content ?? "fuck the api failed.";

            ModelState.SetModelValue("Message", new ValueProviderResult(model.Message));
            return View("Index", model);
        }
        #endregion

        #region VOTING
        public IActionResult Voting()
        {
            var goals = GetStoredGoals() ?? [];
            goals = goals.OrderByDescending(x => x.Rating).ToList();
            goals.ForEach(x => x.Rating = 0);

            var model = new VotingModel()
            {
                RankingIndex = goals!.Count,
                FirstIndex = 0,
                SecondIndex = 1,
                NextIndex = 2,
                Goals = goals,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Voting(VotingModel model, [FromQuery] string choice)
        {
            model.DeserializeGoals();

            Goal? firstGoal = model.Goals.ElementAtOrDefault(model.FirstIndex);
            Goal? secondGoal = model.Goals.ElementAtOrDefault(model.SecondIndex);
            Goal? nextGoal = model.Goals.ElementAtOrDefault(model.NextIndex);

            if (firstGoal == null) throw new Exception("first is null");
            if (secondGoal == null) throw new Exception("second is null");

            if (nextGoal == null)
            {
                // he was number 1
                if (choice == nameof(VotingModel.FirstIndex))
                {
                    secondGoal.Rating = model.RankingIndex;
                    firstGoal.Rating = model.RankingIndex - 1;
                }
                else if (choice == nameof(VotingModel.SecondIndex))
                {
                    secondGoal.Rating = model.RankingIndex;
                    firstGoal.Rating = model.RankingIndex - 1;
                }
                return PostVoting(model);
            }

            if (choice == nameof(VotingModel.FirstIndex))
            {
                secondGoal.Rating = model.RankingIndex;
                model.SecondIndex = model.NextIndex;
            }
            else if (choice == nameof(VotingModel.SecondIndex))
            {
                firstGoal.Rating = model.RankingIndex;
                model.FirstIndex = model.NextIndex;
            }

            model.RankingIndex -= 1;
            model.NextIndex += 1;

            model.GoalsJson = JsonSerializer.Serialize(model.Goals);

            ModelState.SetModelValue("RankingIndex", new ValueProviderResult(model.RankingIndex.ToString()));
            ModelState.SetModelValue("FirstIndex", new ValueProviderResult(model.FirstIndex.ToString()));
            ModelState.SetModelValue("SecondIndex", new ValueProviderResult(model.SecondIndex.ToString()));
            ModelState.SetModelValue("NextIndex", new ValueProviderResult(model.NextIndex.ToString()));
            ModelState.SetModelValue("GoalsJson", new ValueProviderResult(model.GoalsJson));
            return View(model);
        }
        
        public IActionResult PostVoting(VotingModel model)
        {
            string updatedJson = JsonSerializer.Serialize(model.Goals, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_goalsJsonPath, updatedJson);

            return Redirect("Index");
        }
        #endregion

        List<Goal>? GetStoredGoals()
        {
            string json = System.IO.File.ReadAllText(_goalsJsonPath);
            return JsonSerializer.Deserialize<List<Goal>>(json);
        }
    }
}
