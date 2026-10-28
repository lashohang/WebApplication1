using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace WebApplication1.Pages
{
    public class EmployeesModel : PageModel
    {
        public string? Department { get; set; }

        [BindProperty]
        public string? SearchEmployeeNo { get; set; }

        [BindProperty]
        public int EmployeeID { get; set; }

        [BindProperty]
        public string? EmployeeNo { get; set; }

        [BindProperty]
        public string? FirstName { get; set; }

        [BindProperty]
        public string? LastName { get; set; }

        [BindProperty]
        public string? Email { get; set; }

        [BindProperty]
        public string? Phone { get; set; }

        [BindProperty]
        
        public string? Position { get; set; }

        public string? Message { get; set; }

        private readonly string connectionString =
            "Server=localhost;Database=EmployeeDB;Uid=root;Pwd=;";

        public void OnGet()
        {
            Department =
                HttpContext.Session.GetString("Department");

            if (string.IsNullOrEmpty(Department))
            {
                Response.Redirect("/Login");
            }
        }

        public IActionResult OnPostSearch()
        {
            Department =
                HttpContext.Session.GetString("Department");

            if (string.IsNullOrEmpty(Department))
            {
                return RedirectToPage("/Login");
            }

            if (string.IsNullOrWhiteSpace(SearchEmployeeNo))
            {
                Message = "Please enter an Employee No.";
                return Page();
            }

            string query = @"
                SELECT EmployeeID,
                       EmployeeNo,
                       FirstName,
                       LastName,
                       Email,
                       Phone,
                       Position
                FROM Employees
                WHERE EmployeeNo = @EmployeeNo";

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@EmployeeNo",
                        SearchEmployeeNo);

                    try
                    {
                        connection.Open();

                        using (MySqlDataReader reader =
                               command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                EmployeeID =
                                    Convert.ToInt32(
                                        reader["EmployeeID"]);

                                EmployeeNo =
                                    reader["EmployeeNo"].ToString();

                                FirstName =
                                    reader["FirstName"].ToString();

                                LastName =
                                    reader["LastName"].ToString();

                                Email =
                                    reader["Email"].ToString();

                                Phone =
                                    reader["Phone"].ToString();

                                Position =
                                    reader["Position"].ToString();

                                Message = "Employee found.";
                            }
                            else
                            {
                                Message =
                                    "Employee not found.";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Message =
                            "Database error: " + ex.Message;
                    }
                }
            }

            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            Department = HttpContext.Session.GetString("Department");

            if (string.IsNullOrEmpty(Department))
            {
                return RedirectToPage("/Login");
            }

            if (EmployeeID <= 0)
            {
                Message = "Invalid Employee ID. Please search for an employee first.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(EmployeeNo))
            {
                Message = "Employee No cannot be empty.";
                return Page();
            }

            string query = @"
        UPDATE Employees
        SET EmployeeNo = @EmployeeNo,
            FirstName = @FirstName,
            LastName = @LastName,
            Email = @Email,
            Phone = @Phone,
            Position = @Position
        WHERE EmployeeID = @EmployeeID";

            try
            {
                using (MySqlConnection connection =
                       new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand command =
                           new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@EmployeeID", EmployeeID);

                        command.Parameters.AddWithValue(
                            "@EmployeeNo", EmployeeNo ?? "");

                        command.Parameters.AddWithValue(
                            "@FirstName", FirstName ?? "");

                        command.Parameters.AddWithValue(
                            "@LastName", LastName ?? "");

                        command.Parameters.AddWithValue(
                            "@Email", Email ?? "");

                        command.Parameters.AddWithValue(
                            "@Phone", Phone ?? "");

                        command.Parameters.AddWithValue(
                            "@Position", Position ?? "");

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Message =
                                "Employee information updated successfully.";
                        }
                        else
                        {
                            // MySQL may return 0 when the submitted
                            // information is exactly the same.
                            Message =
                                "No changes were made. The employee information may already be up to date.";
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    Message =
                        "This Employee No already exists. Please use a different Employee No.";
                }
                else
                {
                    Message =
                        "MySQL error: " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                Message =
                    "Error: " + ex.Message;
            }

            return Page();
        }
    }
}