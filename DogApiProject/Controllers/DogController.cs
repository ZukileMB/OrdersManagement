using DogApiProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DogApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DogController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DogController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Route("random")]
        public async Task<IActionResult> GetRandomDogImage()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync("https://dog.ceo/api/breeds/image/random");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<DogApiResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return Ok(apiResponse);
            }
            return StatusCode((int)response.StatusCode, "Error fetching dog image.");
        }

    }
}
