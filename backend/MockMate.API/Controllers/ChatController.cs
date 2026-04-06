using Microsoft.AspNetCore.Mvc;
using MockMate.API.Models;
using System.Text;
using System.Text.Json;

namespace MockMate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string SYSTEM_PROMPT = @"
            You are MockMate, an AI interview coach created by Vishal.
            Conduct structured technical interviews based on the user's 
            selected role, skills, and difficulty level (Beginner, 
            Intermediate, Advanced). Ask one question at a time, evaluate 
            answers with a score out of 10, give brief feedback, and at the 
            end provide a full performance report with strengths, weaknesses, 
            and recommended resources.";

        public ChatController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            var messages = new List<object>
            {
                new { role = "system", content = SYSTEM_PROMPT }
            };

           foreach (var history in request.History)
                messages.Add(new { role = history.Role, content = history.Content });

            messages.Add(new { role = "user", content = request.Message });

            var payload = new
            {
                model = "llama3.2:1b",
                messages,
                stream = false
            };

            var response = await _httpClient.PostAsJsonAsync(
                "http://localhost:11434/api/chat", payload);

            var json = await response.Content.ReadAsStringAsync();
             Console.WriteLine("Ollama Raw Response: " + json);
            try
            {
                var result = JsonDocument.Parse(json);
                var root = result.RootElement;
                string? reply = null;

                if (root.TryGetProperty("message", out var messageEl))
                {
                    reply = messageEl.GetProperty("content").GetString();
                }
                else if (root.TryGetProperty("response", out var responseEl))
                {
                    reply = responseEl.GetString();
                }
                else
                {
                    reply = "⚠️ Unexpected response format from Ollama.";
                }


                return Ok(new { reply });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Parse error: " + ex.Message);
                return StatusCode(500, new { reply = "⚠️ Failed to parse Ollama response." });
            }
        }
    }
}