using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace ZooCoder.Pages
{
    public class IndexModel : PageModel
    {
        public List<FeedingSchedule> FeedingSchedules { get; set; } = new();

        // called when load w http get req
        public void OnGet()
        {
            // def connection to sqlite db
            string connectionString = "Data Source=FeedingSchedules.db";

            // open connection to db
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // query
                string query = "SELECT animal_id, name, feeding_time, species, employee_id FROM Animals";

                // exec query
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int employeeId = reader.GetInt32(4);

                        FeedingSchedules.Add(new FeedingSchedule
                        {
                            animal_id = reader.GetInt32(0),
                            name = reader.GetString(1),
                            species = reader.GetString(3),
                            feeding_time = reader.GetString(2),
                            employee_id = employeeId, // assign id from animals table to var for method arg
                            last_name = GetEmployeeNameById(employeeId)
                        });
                    }
                }
            }
        }

        // return emplyee name using their id
        public string GetEmployeeNameById(int employee_id)
        {
            // open new connection
            using (var connection = new SqliteConnection("Data Source=FeedingSchedules.db"))
            {
                connection.Open();

                // query
                using (var command = new SqliteCommand("SELECT last_name FROM Employees WHERE employee_id = @employee_id", connection))
                {
                    command.Parameters.AddWithValue("@employee_id", employee_id); // parameter for employee_id

                    using (var reader = command.ExecuteReader())
                    {
                        // if found
                        if (reader.Read()) 
                        {
                            return reader.GetString(0);
                        }
                    }
                }
            }

            return "Unknown"; // if no employee found
        }
    }

    // model class
    // store data for each animal
    public class FeedingSchedule
    {
        public int animal_id { get; set; }
        public string name { get; set; }
        public string feeding_time { get; set; }
        public int employee_id { get; set; }
        public string last_name { get; set; }
        public string species { get; set; }
    }

}
