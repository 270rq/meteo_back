using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace meteo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WeatherWeek : ControllerBase
        
    {
        
        public static List<(DateTime,int)> GetAllValuesFromMenuTable(int City, DateTime Day)
        {
            string connectionString = "server=localhost;user id=root;password=1111;database=meteo";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            DateTime endDate = Day.AddDays(7);
            string query = "SELECT * FROM menu WHERE city_id = " + City.ToString() + " AND id >= '" + Day.ToString("yyyy-MM-dd") + "' AND id <= '" + endDate.ToString("yyyy-MM-dd") + "'";

            MySqlCommand command = new MySqlCommand(query, connection);

            MySqlDataReader reader = command.ExecuteReader();

            List<(DateTime,int)> values = new List<(DateTime,int)>();

            while (reader.Read())
            {
                var value = (reader.GetDateTime(1),reader.GetInt16(2));

                values.Add(value);
            }
            reader.Close();
            connection.Close();

            return values;

        }
        [HttpGet("/WeatherWeek")]
        public List<meteo.WeatherWeek> Get(DateTime Day, int City)
        {
            List<meteo.WeatherWeek> weatherDays = new List<meteo.WeatherWeek>();
            List<(DateTime, int)> valuesList1 = GetAllValuesFromMenuTable(City, Day);
            foreach (var values in valuesList1)
            {
                meteo.WeatherWeek weatherWeek = new meteo.WeatherWeek
                {
                    day = values.Item1,
                    TemperatureC = values.Item2,
                };
                weatherDays.Add(weatherWeek);
            }
            return weatherDays;
        }
    }
   
}

