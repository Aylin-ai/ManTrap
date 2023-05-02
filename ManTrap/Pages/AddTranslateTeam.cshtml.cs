using ManTrap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace ManTrap.Pages
{
    public class AddTranslateTeamModel : PageModel
    {
        public bool IsUserHasTeam { get; set; }
        public string TeamName { get; set; }
        public string UserRole { get; set; }
        public string DateOfCreation { get; set; }

        public void OnGet()
        {
            GetMyTeam();
        }

        public async Task<IActionResult> OnPostAddTranslateTeam(string translateTeamName)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "insert into translateteam (Title, DateOfCreation) values " +
                    "(@translateTeam, @dateOfCreation);";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                cmd.Parameters.AddWithValue("@translateTeam", translateTeamName);
                cmd.Parameters.AddWithValue("@dateOfCreation", DateTime.Today.ToString("yyyy-MM-dd"));

                await cmd.ExecuteNonQueryAsync();

                sql = "select Id from translateteam where Title = @translateTeam";
                cmd.CommandText = sql;

                int Id = 0;

                var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Id = reader.GetInt32(0);
                    }
                }
                reader.Close();

                sql = "insert into translateteamcomposition values " +
                    "(@id, @login, 1)";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Parameters.AddWithValue("@login", User.Identity.Name);
                await cmd.ExecuteNonQueryAsync();
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Page();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        void GetMyTeam()
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "select translateteamrole.Title, " +
                    "translateteamcomposition.UserInformation_Login, " +
                    "translateteam.Title, translateteam.DateOfCreation " +
                    "from translateteamcomposition inner join translateteam " +
                    "on translateteamcomposition.TranslateTeam_Id = translateteam.Id " +
                    "inner join translateteamrole " +
                    "on translateteamcomposition.TranslateTeamRole_Id = translateteamrole.Id " +
                    "where translateteamcomposition.UserInformation_Login = @user;";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@user", User.Identity.Name);

                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        IsUserHasTeam = true;
                        UserRole = reader.GetString(0);
                        TeamName = reader.GetString(2);
                        DateOfCreation = reader.GetDateTime(3).ToString("yyyy-MM-dd");
                    }
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
