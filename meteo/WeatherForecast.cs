using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.ComponentModel.DataAnnotations;

namespace meteo
{
    public class SunData
    {
        public DateTime? Day { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public TimeSpan SunRise { get; set; }
        public TimeSpan SunSet { get; set; }
    }
    public class WeatherData
    {
        public DateTime? Day { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public int Temperature { get; set; }
        public float Precipitation { get; set; }
        public float Uv_index { get; set; }
        public float Wind_speed { get; set; }
        public string Wind_direction { get; set; }
        public float Visibility { get; set; }
    }
    public class WeatherDay
    {
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
        public string Visi_state { get; set; }
        public string WeatherSensation { get; set; }
    }

    public class WeatherWeek
    {
        public DateTime day { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }

    public class Flower_map
    {

        public string? NameFlower { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string? Lvl { get; set; }
    }
    public class RegUserModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class LogUserModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public class cityWithRegion
    {
        public string City { get; set; }
        public string Region { get; set; } 
    }
}