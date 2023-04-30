using ManTrap.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Net.Http;

namespace ManTrap.Pages
{
    public class MangaIDModel : PageModel
    {
        [BindProperty(Name = "mangaId", SupportsGet = true)]
        public int Id { get; set; }
        public Manga Manga { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                await GetMangas();
                return Page();
            }
            else
                return RedirectToPage("Index");
        }

        public IActionResult OnPostMangaIdPage(int id)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToPage("/MangaId", new { mangaId = id });
            else
                return RedirectToPage("Index");
        }

        private async Task GetMangas()
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                string sql = "select manga.Id, manga.ImageSource, manga.RussianName, manga.EnglishName, " +
                    "mangatype.Title, mangastatus.Title, translatestatus.Title " +
                    "manga.YearOfRelease, manga.Overview " +
                    "from manga inner join mangatype " +
                    "on manga.MangaType_Id = mangatype.Id " +
                    "inner join mangastatus " +
                    "on manga.MangaStatus_Id = mangastatus.Id" +
                    "inner join translatestatus " +
                    "on manga.TranslateStatus.Id = translatestatus.Id " + 
                    "where manga.Id = @Id and manga.Checked = 1;";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@Id", Id);

                var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Manga = new Manga()
                        {
                            Id = reader.GetInt32(0),
                            ImageSource = reader.GetString(1),
                            RussianName = reader.GetString(2),
                            EnglishName = reader.GetString(3),
                            Type = reader.GetString(4),
                            Status = reader.GetString(5),
                            TranslateStatus = reader.GetString(6),
                            YearOfRelease = reader.GetInt32(7),
                            Overview = reader.GetString(8),
                        };
                    }
                }
                reader.Close();

                sql = "select mangagenres.Manga_Id, genre.Title " +
                    "from mangagenres inner join genre " +
                    "on mangagenres.Genre_Id = genre.Id;";
                cmd.CommandText = sql;

                reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (Manga.Id == reader.GetInt32(0))
                        {
                            Manga.Genres.Add(reader.GetString(1));
                        }
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
