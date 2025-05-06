namespace ChefKnifeStudios.GrowYourDad.GoalDesigner.Models;

public class ChatGPTRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "gpt-4";

    [JsonPropertyName("messages")]
    public List<ChatGPTMessage> Messages { get; set; } = new();

    [JsonPropertyName("temperature")]
    public float Temperature { get; set; } = 0.7f;
}

public class ChatGPTMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = "user";

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class ChatGPTResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("object")]
    public string Object { get; set; } = string.Empty;

    [JsonPropertyName("created")]
    public int Created { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("choices")]
    public List<ChatGPTChoice> Choices { get; set; } = new();
}

public class ChatGPTChoice
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("logprobs")]
    public object? LogProbs { get; set; }

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; } = string.Empty;
}

