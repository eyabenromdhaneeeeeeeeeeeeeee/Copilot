using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Copilot_Crm.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Copilot_Crm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ChatController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://hospitality-aiservice-apimanagement.azure-api.net/");
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "a4d0a552cd394456a1902254f610b35b");
            _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        }

        [HttpPost("SendMessageToChat")]
        public async Task<IActionResult> SendMessageToChat([FromBody] ChatRequestModel requestModel)
        {
            if (requestModel == null || requestModel.Messages == null || requestModel.Messages.Count == 0)
            {
                return BadRequest("Invalid request payload.");
            }

            try
            {
                var chatRequest = new
                {
                    model = "gpt-3.5-turbo-0613",
                    messages = requestModel.Messages.Select(m => new
                    {
                        role = m.Role,
                        content = m.Content
                    }).ToArray()
                };

                var json = JsonConvert.SerializeObject(chatRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Acteol/deployments/Acteol/chat/completions", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Ok(responseContent);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}