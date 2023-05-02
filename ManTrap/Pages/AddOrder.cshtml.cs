using ManTrap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace ManTrap.Pages
{
    public class AddOrderModel : PageModel
    {
        public List<SelectListItem> Mangas { get; set; }

        public AddOrderModel()
        {
            GetManga();
        }

        public void OnGet()
        {
        }

        public IActionResult OnPostUpload(int cost, DateOnly targetDate, int[] mangaId, string[] sourceOfChapterToTranslate, int[] chapterId)
        {
            if (!ModelState.IsValid)
                return Page();

            if (cost == 0 || mangaId == null || sourceOfChapterToTranslate == null || chapterId == null)
            {
                return BadRequest("Вы не ввели все данные");
            }
            if (mangaId.Length != sourceOfChapterToTranslate.Length &&
                sourceOfChapterToTranslate.Length != chapterId.Length &&
                mangaId.Length != chapterId.Length)
            {
                return BadRequest("Вы не ввели все данные в составе заказа");
            }

            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                string sql = "insert into contract (DateOfCreation, ContractStatus_Id, UserInformation_Login_Customer, " +
                    "TranslateTeam_Id, Cost, TargetDate, Paid) values " +
                    "(@dateOfCreation, @contractStatusId, @customerLogin, @translateTeamId, @cost, @targetDate, @paid)";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@dateOfCreation", DateTime.Today.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@contractStatusId", 1);
                cmd.Parameters.AddWithValue("@customerLogin", User.Identity.Name);
                cmd.Parameters.AddWithValue("@translateTeamId", 1);
                cmd.Parameters.AddWithValue("@cost", cost);
                cmd.Parameters.AddWithValue("@targetDate", targetDate.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@paid", 0);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                int orderId = 0;

                sql = "select max(Id) from contract";
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        orderId = reader.GetInt32(0);
                    }
                }
                reader.Close();

                sql = "insert into contractcomposition (Contract_Id, Manga_Id, SourceOfChapterToTranslate, " +
                    "ChapterId, Accepted) values " +
                    "(@contractId, @mangaId, @sourceOfChapter, @chapterId, 0)";
                cmd.CommandText = sql;
                for (int i = 0; i < mangaId.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@contractId", orderId);
                    cmd.Parameters.AddWithValue("@mangaId", mangaId[i]);
                    cmd.Parameters.AddWithValue("@sourceOfChapter", sourceOfChapterToTranslate[i]);
                    cmd.Parameters.AddWithValue("@chapterId", chapterId[i]);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                return Page();
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

        public void GetManga()
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                string sql = "select manga.Id, manga.RussianName from manga";
                cmd.CommandText = sql;

                var reader = cmd.ExecuteReader();

                List<SelectListItem> list = new List<SelectListItem>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = reader.GetInt32(0).ToString(),
                            Text = reader.GetString(1),
                        });
                    }
                }
                Mangas = list;
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
