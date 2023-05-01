using ManTrap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace ManTrap.Pages
{
    public class AddArtistModel : PageModel
    {
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAddArtist(string artistName, string artistOverview)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "insert into artist (Title) values (@artist);";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                cmd.Parameters.AddWithValue("@artist", artistName);

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
