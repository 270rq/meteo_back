using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Net;

namespace MyNamespace.Controllers
{
    public class WeatherDay
    {
        public DateTime Day { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public float Precipitation { get; set; }
        public string? TypeWind { get; set; }
        public float SpeedWind { get; set; }
        public float UvIndex { get; set; }
        public int Visibility { get; set; }
        public TimeSpan Sunrise { get; set; }
        public TimeSpan Sunset { get; set; }
        public int DewPoint { get; set; }
        public string WeatherSensation { get; set; }
    }

    public class Flower_map
    {
        public string? Month { get; set; }
        public string? NameFlower { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }

    [ApiController]
    [Route("get")]
    public class MyController2 : ControllerBase
    {
        private readonly ILogger<MyController> _logger;
        private readonly HttpClient _httpClient;

        public MyController2(ILogger<MyController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/WeatherDay?Day=2023-10-09&City=%D0%9A%D0%B8%D1%80%D0%BE%D0%B2&Region=%D0%9A%D0%B8%D1%80%D0%BE%D0%B2%D1%81%D0%BA%D0%B0%D1%8F%20%D0%BE%D0%B1%D0%BB%D0%B0%D1%81%D1%82%D1%8C");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<object>(content, options);

                return Ok("All is OK"); // Return success message
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization failed");
                return StatusCode(500, "Error occurred while deserializing JSON");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpGet("T_check")]
        public async Task<IActionResult> T_check()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/WeatherDay?Day=2023-10-09&City=Киров&Region=Кировская область");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<WeatherDay>>(content, options);

                var temperature = result[0];
                if (temperature.TemperatureC is int temp && (temp < -100 || temp > 100))
                {
                    return BadRequest("Invalid temperature value");
                }



                return Ok("All is OK"); // Вернуть успешное сообщение
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making the HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed");
                return BadRequest("Invalid JSON data");
            }
        }
        [HttpGet("Wind_Type_check")]
        public async Task<IActionResult> Wind_Type_check()
        {
            string[] compassDirections = new string[] { "N", "S", "E", "W", "NE", "SE", "NW", "SW" };

            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/WeatherDay?Day=2023-10-09&City=Киров&Region=Кировская область");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<WeatherDay>>(content, options);
                if (!compassDirections.Contains(result[0].TypeWind))
                {
                    return BadRequest("Invalid typeWind value");
                }



                return Ok("All is OK"); // Вернуть успешное сообщение
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making the HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed");
                return BadRequest("Invalid JSON data");
            }
        }
        [HttpGet("Uv_check")]
        public async Task<IActionResult> Uv_check()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/WeatherDay?Day=2023-10-09&City=Киров&Region=Кировская область");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<WeatherDay>>(content, options);


                // Check if UvIndex is within the range of 0 to 12
                if (result[0].UvIndex < 0 || result[0].UvIndex > 12)
                {
                    return BadRequest("UvIndex is out of the valid range (0-12)");
                }

                return Ok("All is OK");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making the HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed");
                return BadRequest("Invalid JSON data");
            }
        }
        [HttpGet("Visibility_check")]
        public async Task<IActionResult> Visibility_check()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/WeatherDay?Day=2023-10-09&City=Киров&Region=Кировская область");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<WeatherDay>>(content, options);



                if (result[0].Visibility >= 0 || result[0].Visibility <= 200)
                {
                    return BadRequest("Visibility is out of the valid range (0-200)");
                }

                return Ok("All is OK");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making the HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed");
                return BadRequest("Invalid JSON data");
            }
        }
        [HttpGet("DewPoint_check")]
        public async Task<IActionResult> DewPoint_check()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/WeatherDay?Day=2023-10-09&City=Киров&Region=Кировская область");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<WeatherDay>>(content, options);



                if (result[0].DewPoint >= 0 || result[0].DewPoint <= 35)
                {
                    return BadRequest("Dew Point is out of the valid range (0-35)");
                }

                return Ok("All is OK");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making the HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed");
                return BadRequest("Invalid JSON data");
            }
        }
        [HttpGet("S_S_check")]
        public async Task<IActionResult> S_S_check()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/WeatherDay?Day=2023-10-09&City=Киров&Region=Кировская область");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<WeatherDay>>(content, options);



                if (result[0].Sunrise == result[0].Sunset)
                {
                    return BadRequest("Sunrise and sunset cannot be equal");
                }

                return Ok("All is OK");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making the HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed");
                return BadRequest("Invalid JSON data");
            }
        }
        [HttpGet("X_Y_check")]
        public async Task<IActionResult> X_Y_check()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/Map?Month=June&Name_flower=brich&X=12&Y=20");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<Flower_map>>(content, options);



                if (result[0].X >= -90 && result[0].X <= 90 && result[0].Y >= -180 && result[0].Y <= 180)
                {
                    return BadRequest("X and Y are out of the valid range");
                }

                return Ok("All is OK");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making the HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed");
                return BadRequest("Invalid JSON data");
            }
        }
        [HttpGet("Mounth_check")]
        public async Task<IActionResult> Mounth_check()

        {
            string[] calendarMounth = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/Map?Month=June&Name_flower=brich&X=12&Y=20");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<Flower_map>>(content, options);



                if (!calendarMounth.Contains(result[0].Month))
                {
                    return BadRequest("Invalid Month value");
                }

                return Ok("All is OK");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making the HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed");
                return BadRequest("Invalid JSON data");
            }
        }

        [HttpGet("n_flower_check")]
        public async Task<IActionResult> n_flower_check()

        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/Map?Month=June&Name_flower=brich&X=12&Y=20");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<Flower_map>>(content, options);



                if (result == null)
                {
                    return BadRequest("Invalid flower value");
                }

                return Ok("All is OK");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making the HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed");
                return BadRequest("Invalid JSON data");
            }
        }
        [HttpGet("mounth_check1")]
        public async Task<IActionResult> mounth_check1()

        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/Map?Month=June&Name_flower=brich&X=12&Y=20");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<Flower_map>>(content, options);



                if (result == null)
                {
                    return BadRequest("Invalid mounth value");
                }

                return Ok("All is OK");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making the HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed");
                return BadRequest("Invalid JSON data");
            }
        }
        [HttpGet("x_y_check1")]
        public async Task<IActionResult> x_y_check1()

        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/Map?Month=June&Name_flower=brich&X=12&Y=20");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<Flower_map>>(content, options);
                foreach (var map in result)
                {
                    if (map.X == null && map.Y == null)
                    {
                        return BadRequest("Invalid x and y value");
                    }
                }

                return Ok("All is OK");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making the HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed");
                return BadRequest("Invalid JSON data");
            }
        }
        [HttpGet("n_flower_check1")]
        public async Task<IActionResult> n_flower_check1()

        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/Map?Month=June&Name_flower=brich&X=12&Y=20");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<Flower_map>>(content, options);


                if (result.Count == 0)
                {
                    return Ok("All is OK");
                }


                return BadRequest("Null value");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making the HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed");
                return BadRequest("Invalid JSON data");
            }
        }
        [HttpGet("check_two_days")]
        public async Task<IActionResult> CheckTwoDaysWeatherAvailability()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/WeatherDay?Day=2023-10-09&City=Киров&Region=Кировская область");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var response1 = await _httpClient.GetAsync("https://localhost:7024/WeatherDay?Day=2023-10-10&City=Киров&Region=Кировская область");

                response1.EnsureSuccessStatusCode();

                var content1 = await response1.Content.ReadAsStringAsync();

                if (response != null && response1 != null)
                {
                    return Ok("2 days of weather are available");
                }
                else
                {
                    return BadRequest("2 days of weather are not available");
                }

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making the HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed");
                return BadRequest("Invalid JSON data");
            }
        }

        [HttpGet("check_x_without_y")]
        public async Task<IActionResult> check_x_without_y()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7024/Map?Month=June&Name_flower=brich&X=12");
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return Ok("All is OK");
                }
                return BadRequest("Not OK");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
                return StatusCode(500, "Error occurred while making the HTTP request");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing failed");
                return BadRequest("Invalid JSON data");
            }
        }
    }


