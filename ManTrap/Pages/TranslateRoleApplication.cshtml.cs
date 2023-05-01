using ManTrap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pkcs;

namespace ManTrap.Pages
{
    public class TranslateRoleApplicationModel : PageModel
    {
        [BindProperty(Name = "login", SupportsGet = true)]
        public string Login { get; set; }

        public bool IsApplicationAlreadyExists { get; set; }
        public bool IsApplicationAccepted { get; set; }

        public TranslateRoleApplicationModel()
        {
        }

        public void OnGet()
        {
            CheckUsersApplicationExistance();
        }

        public IActionResult OnPostAddApplication(string exampleWork)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                string sql = "insert into translatorapplication " +
                    "values (@login, @date, @ExampleWork, 0)";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@login", User.Identity.Name);
                cmd.Parameters.AddWithValue("@date", DateTime.Today.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("ExampleWork", exampleWork);
                cmd.ExecuteNonQuery();
                return RedirectToPage("/UserProfile", new { message = "Ваша заявка подана. Ожидайте одобрения или отказа" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public void CheckUsersApplicationExistance()
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                string sql = "select * from translatorapplication where UserInformation_Login = @login";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@login", User.Identity.Name);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    IsApplicationAlreadyExists = true;
                    while (reader.Read())
                    {
                        IsApplicationAccepted = reader.GetBoolean(3);
                    }
                }
                else
                {
                    IsApplicationAlreadyExists = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
    }
}
