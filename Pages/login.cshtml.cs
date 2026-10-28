using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace WebApplication1.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter username and password.";
                return Page();
            }

            string connectionString =
                "Server=localhost;" +
                "Database=EmployeeDB;" +
                "Uid=root;" +
                "Pwd=;";

            string query =
                "SELECT AdminID, Username, Department " +
                "FROM Admins " +
                "WHERE Username = @Username " +
                "AND Password = @Password";

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Username", Username);

                    command.Parameters.AddWithValue(
                        "@Password", Password);

                    try
                    {
                        connection.Open();

                        using (MySqlDataReader reader =
                               command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                HttpContext.Session.SetString(
                                    "AdminID",
                                    reader["AdminID"].ToString());

                                HttpContext.Session.SetString(
                                    "Username",
                                    reader["Username"].ToString());

                                HttpContext.Session.SetString(
                                    "Department",
                                    reader["Department"].ToString());

                                return RedirectToPage("/Employees");
                            }
                            else
                            {
                                ErrorMessage =
                                    "Invalid username or password.";

                                return Page();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage =
                            "Database error: " + ex.Message;

                        return Page();
                    }
                }
            }
        }
    }
}