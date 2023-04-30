using ManTrap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;

namespace ManTrap.Pages
{
    public class AddAuthorModel : PageModel
    {
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAddAuthor(string authorName, string authorOverview)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "insert into author (Title) values (@author);";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                cmd.Parameters.AddWithValue("@author", authorName);

                await cmd.ExecuteNonQueryAsync();
                return RedirectToPage("AddManga");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToPage("AddManga");
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
    }
}
