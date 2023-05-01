using ManTrap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace ManTrap.Pages
{
    public class AddPublisherModel : PageModel
    {
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAddPublisher(string publisherName, string publisherOverview)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "insert into publisher (Title) values (@publisher);";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                cmd.Parameters.AddWithValue("@publisher", publisherName);

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
