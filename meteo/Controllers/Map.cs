using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace meteo.Controllers
{
    [ApiController]
    public class MapController : ControllerBase
    {
        private string connectionString = "server=localhost;user id=root;password=1111;database=meteo";

        [HttpGet("/Map")]
        public List<FlowerMap> Get(DateTime day, string nameFlower)
        {
            List<FlowerMap> map = new List<FlowerMap>();
            List<(DateTime, float, float, string, string, string)> valuesList1 = GetAllValuesFromMapTable(day, nameFlower);
            if (valuesList1 == null)
            {
                return null;
            }
            foreach (var values in valuesList1)
            {
                FlowerMap flowerMap = new FlowerMap()
                {
                    day = values.Item1,
                    NameFlower = values.Item4,
                    X = values.Item2,
                    Y = values.Item3,
                    Lvl = values.Item5,
                    Family = values.Item6,
                };
                map.Add(flowerMap);
            }
            return map;
        }

        [HttpGet("/Family")]
        public List<string> GetFamilies()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM family";
                MySqlCommand command = new MySqlCommand(query, connection);
                List<string> values = new List<string>();

                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string value = reader.GetString(0);
                            values.Add(value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return null;
                }

                return values;
            }
        }

        [HttpGet("/Plants")]
        public List<string> GetPlants(string family)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM flower WHERE family = @family";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@family", family);
                List<string> values = new List<string>();

                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string value = reader.GetString(0);
                            values.Add(value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return null;
                }

                return values;
            }
        }

        [HttpGet("/AllPlants")]
        public List<string> GetAllPlants()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM flower";
                MySqlCommand command = new MySqlCommand(query, connection);
                List<string> values = new List<string>();

                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string value = reader.GetString(0);
                            values.Add(value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return null;
                }

                return values;
            }
        }

        [HttpGet("/City")]
        public List<CityWithRegion> GetCityWithRegion()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM cityWithRegion";
                MySqlCommand command = new MySqlCommand(query, connection);
                List<CityWithRegion> values = new List<CityWithRegion>();

                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CityWithRegion value = new CityWithRegion()
                            {
                                City = reader.GetString(1),
                                Region = reader.GetString(2)
                            };
                            values.Add(value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return null;
                }

                return values;
            }
        }

        private List<(DateTime, float, float, string,string,string)> GetAllValuesFromMapTable(DateTime day, string nameFlower)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT map.*, flower.family FROM map JOIN flower ON map.name_flower = flower.name WHERE map.name_flower = @nameFlower AND (map.day = @day OR map.day = @nextday)";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@nameFlower", nameFlower);
                command.Parameters.AddWithValue("@day", day);
                command.Parameters.AddWithValue("@nextday",day.AddDays(1));
                Console.WriteLine(query);
                Console.WriteLine(day);
                List<(DateTime, float, float, string,string,string)> values = new List<(DateTime, float, float, string, string, string)>();

                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var value = (reader.GetDateTime(0), reader.GetFloat(2), reader.GetFloat(3), reader.GetString(1), reader.GetString(4),reader.GetString(5));
                            values.Add(value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return null;
                }

                return values;
            }
        }
    }

    public class FlowerMap
    {
        public DateTime day {  get; set; }
        public string NameFlower { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string Lvl { get; set; } 
        public string Family { get; set; }
    }

    public class CityWithRegion
    {
        public string City { get; set; }
        public string Region { get; set; }
    }
}