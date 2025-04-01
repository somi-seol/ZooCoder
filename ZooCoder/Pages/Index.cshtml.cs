using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace ZooCoder.Pages
{
    public class IndexModel : PageModel
    {
        public List<FeedingSchedule> FeedingSchedules { get; set; } = new();

        public void OnGet()
        {
            string connectionString = "Data Source=FeedingSchedules.db";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT animal_id, name, feeding_time, employee_id FROM Animals";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int employeeId = reader.GetInt32(3);
                        FeedingSchedules.Add(new FeedingSchedule
                        {
                            animal_id = reader.GetInt32(0),
                            name = reader.GetString(1),
                            feeding_time = reader.GetString(2),
                            employee_id = employeeId,
                            last_name = GetEmployeeNameById(employeeId)
                        });
                    }
                }
            }
        }


        public string GetEmployeeNameById(int employee_id)
        {
            using (var connection = new SqliteConnection("Data Source=FeedingSchedules.db"))
            {
                connection.Open();
                using (var command = new SqliteCommand("SELECT last_name FROM Employees WHERE employee_id = @employee_id", connection))
                {
                    command.Parameters.AddWithValue("@employee_id", employee_id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read()) 
                        {
                            return reader.GetString(0);
                        }
                    }
                }
            }

            return "Unknown"; // Return a default value if no employee is found
        }
    }

    public class FeedingSchedule
{
    public int animal_id { get; set; }
    public string name { get; set; }
    public string feeding_time { get; set; }
    public int employee_id { get; set; }
    public string last_name { get; set; } // Add this property
}

}
