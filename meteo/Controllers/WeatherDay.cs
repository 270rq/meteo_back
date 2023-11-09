using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;

namespace meteo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherDay : ControllerBase
    {
        public static List<(DateTime, TimeSpan, TimeSpan)> GetAllValuesFromSunTable(String City, String Region, DateTime Day)
        {
            string connectionString = "server=localhost;user id=root;password=1111;database=meteo";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            string city_idQ = "SELECT  * FROM cityWithRegion WHERE name= '" + City + "' and region_name = '" + Region + "' ;";
                MySqlCommand command = new MySqlCommand(city_idQ, connection);

            MySqlDataReader reader = command.ExecuteReader();
            int city_id = 3;
            if (reader.Read())
            {
                city_id = reader.GetInt32(0);
                // use the city_id value here
            }

            reader.Close();
            string query = "SELECT * FROM sun where city_id=" + city_id.ToString() + " and date='" + Day.ToString("yyyy-MM-dd") +"'";
             command = new MySqlCommand(query, connection);

            reader = command.ExecuteReader();

            List<(DateTime, TimeSpan, TimeSpan)> values = new List<(DateTime, TimeSpan, TimeSpan)>();

            while (reader.Read())
            {
                var value = (reader.GetDateTime(0), reader.GetTimeSpan(3), reader.GetTimeSpan(2));
                values.Add(value);
            }

            reader.Close();
            connection.Close();

            return values;
        }
         public static List<(DateTime, int, float,float, float, string, int)> GetAllValuesFromMenuTable(string City, string Region, DateTime Day)
        {
            string connectionString = "server=localhost;user id=root;password=1111;database=meteo";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            string city_idQ = "SELECT  * FROM cityWithRegion WHERE name= '" + City + "' and region_name = '" + Region + "' ;";
                MySqlCommand command = new MySqlCommand(city_idQ, connection);

            MySqlDataReader reader = command.ExecuteReader();
            int city_id = 3;
            if (reader.Read())
            {
                city_id = reader.GetInt32(0);
                // use the city_id value here
            }

            reader.Close();
            string query = "SELECT * FROM menu where city_id=" + city_id.ToString()+" and Date(id)='" +Day.ToString("yyyy-MM-dd") +"'";

            command = new MySqlCommand(query, connection);

            reader = command.ExecuteReader();

            List<(DateTime, int, float, float,float, string, int)> values = new List<(DateTime, int, float, float,float, string, int)>();

            while (reader.Read()) 
            {
                var value = (reader.GetDateTime(0), reader.GetInt16(2), reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5), reader.GetString(6), reader.GetInt16(7));
                
                values.Add(value);
            }
            reader.Close();
            connection.Close();

            return values;
        }




        private readonly ILogger<WeatherDay> _logger;

        public WeatherDay(ILogger<WeatherDay> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "WeatherDay")]
        public List<meteo.WeatherDay> Get(DateTime Day, string City, string Region)
        {
            List<meteo.WeatherDay> weatherDays = new List<meteo.WeatherDay>();
            List<(DateTime, TimeSpan, TimeSpan)> valuesList = GetAllValuesFromSunTable(City, Region, Day);
            List<(DateTime, int, float, float, float, string, int)> valuesList1 = GetAllValuesFromMenuTable(City,Region, Day);
            try
            {
                foreach (var values in valuesList1)
                {
                    meteo.WeatherDay weatherDay = new meteo.WeatherDay
                    {
                        Day = values.Item1,
                        TemperatureC = values.Item2,
                        Precipitation = values.Item3,
                        UvIndex = values.Item4,
                        SpeedWind = values.Item5,
                        TypeWind = values.Item6,
                        Visibility = values.Item7,
                        Sunrise = valuesList[0].Item2,
                        Sunset = valuesList[0].Item3,
                        DewPoint = CalculateDewPoint(values.Item2, values.Item3),
                        WeatherSensation = GetWeatherSensation(values.Item2, values.Item3, values.Item5),
                    };
                    weatherDays.Add(weatherDay);
                }
                return weatherDays;
            }
            catch
            {
                return null;
            }
        }

        public static int CalculateDewPoint(double temperature, double precipitation)
        {
            double a = 17.27;
            double b = 237.7;
            double c = 273.15;

            double gamma = Math.Log(precipitation / 100) + (a * temperature) / (b + temperature);
            double dewPoint = (b * gamma) / (a - gamma) - c;
            int result = (int)Math.Round(dewPoint);
            return result;
        }
        public static string GetWeatherSensation(double temperature, double precipitation, double windSpeed)
        {
            double heatIndex = CalculateHeatIndex(temperature, precipitation);
            double windChill = CalculateWindChill(temperature, windSpeed);

            if (heatIndex >= 40)
            {
                return "Очень жарко: высокая температура и влажность";
            }
            else if (windChill <= -27)
            {
                return "Очень холодно: низкая температура и сильный ветер";
            }
            else if (temperature >= 30)
            {
                return "Жарко: высокая температура";
            }
            else if (temperature >= 20)
            {
                return "Тепло: комфортная температура";
            }
            else if (temperature >= 10)
            {
                return "Прохладно: низкая температура";
            }
            else 
            {
                return "Холодно: очень низкая температура";
            }
        }

        private static double CalculateHeatIndex(double temperature, double precipitation)
        {
            double c1 = -8.78469475556;
            double c2 = 1.61139411;
            double c3 = 2.33854883889;
            double c4 = -0.14611605;
            double c5 = -0.012308094;
            double c6 = -0.0164248277778;
            double c7 = 0.002211732;
            double c8 = 0.00072546;
            double c9 = -0.000003582;

            double T = temperature;
            double RH = precipitation;

            double heatIndex = c1 + c2 * T + c3 * RH + c4 * T * RH + c5 * T * T + c6 * RH * RH + c7 * T * T * RH + c8 * T * RH * RH + c9 * T * T * RH * RH;

            return heatIndex;
        }

        private static double CalculateWindChill(double temperature, double windSpeed)
        {
            double V = windSpeed;
            double T = temperature;

            double windChill = 13.12 + 0.6215 * T - 11.37 * Math.Pow(V, 0.16) + 0.3965 * T * Math.Pow(V, 0.16);

            return windChill;
        }

    }
}