using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Copilot_Crm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public CustomerController(IConfiguration configuration)
        {
            _apiKey = configuration["cKVXw7FDXP1pS_Hq8CPkOtru3WPyk1FXEznYM9IhuzNUI8w0zXnVA8kJhOIe7U-a7iQ1N-t_KvzB_du491S1KiC_w6tbwyTVwELZWgfVRRuvYxYNJVKRk23rAkduspYhfJ2k8CEs5wyBOrs2sFKQ2VtQXhuJuNDGRfZZL3vr4h5W44qHiCqnEhsutJZULCPiEXLxtm5tt4ebFFaFhNCIVmGZF0TFXBVd30q5lOH2qi7ZNX0pjE6SMgjyxdtNIZQN-5v8xBhF_kIv0jgXmu0xWGKNfBfCRPieEC984tW6Kna-Fy81EqZd9LtDWXonP69NfjBE33vrbzTCeEbi8Pr1WKgtAyyup5JcYxbiQn9xn73sfMqy"];
            _apiUrl = configuration["https://testonebrand.atreemo.com/api/Customer/FindCustomerDetails"];
            _httpClient = new HttpClient { BaseAddress = new Uri("https://testonebrand.atreemo.com/api/") };
        }

        [HttpPost("FindCustomerDetails")]
        public async Task<IActionResult> FindCustomerDetails([FromBody] FindCustomerDetailsRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                return BadRequest("The customer name field is required.");
            }

            try
            {
                var requestData = new { SearchFilters = new { FirstName = request.CustomerName } };
                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                var response = await _httpClient.PostAsync(_apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var customerDetailsResponse = JsonConvert.DeserializeObject<CustomerDetailsResponseModel>(responseContent);
                    return Ok(customerDetailsResponse);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Failed to find customer details");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class FindCustomerDetailsRequestModel
    {
        public string CustomerName { get; set; }
    }

    public class CustomerDetailsResponseModel
    {
        public List<CustomerDetails> ResponseData { get; set; }
        public bool ResponseStatus { get; set; }
        public List<string> Errors { get; set; }
    }

    public class CustomerDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
    }
}