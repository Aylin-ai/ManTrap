using ManTrap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;

namespace ManTrap.Pages
{
    public class IndexModel : PageModel
    {
        public List<Manga> Mangas { get; set; } = new List<Manga>();

        public List<SelectListItem> Orders { get; set; } = new List<SelectListItem>();
        public string Order { get; set; }

        public List<SelectListItem> Kinds { get; set; } = new List<SelectListItem>();
        public string Kind { get; set; }

        public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
        public string Status { get; set; }

        public List<SelectListItem> Genres { get; set; } = new List<SelectListItem>();
        public int Genre { get; set; }

        public string Search { get; set; }


        public async void OnGet()
        {
            await GetMangas();
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
                    "mangatype.Title, mangastatus.Title " +
                    "from manga inner join mangatype " +
                    "on manga.MangaType_Id = mangatype.Id " +
                    "inner join mangastatus " +
                    "on manga.MangaStatus_Id = mangastatus.Id;";
                cmd.CommandText = sql;

                var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Mangas.Add(
                            new Manga()
                            {
                                Id = reader.GetInt32(0),
                                ImageSource = reader.GetString(1),
                                RussianName = reader.GetString(2),
                                EnglishName = reader.GetString(3),
                                Type = reader.GetString(4),
                                Status = reader.GetString(5),
                            }
                            );
                    }
                }
                reader.Close();

                sql = "select mangagenres.Manga_Id, genre.Title " +
                    "from mangagenres inner join genre " +
                    "on mangagenres.Genre_Id = genre.Id;";
                cmd.CommandText= sql;

                reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        foreach (var manga in Mangas)
                        {
                            if (manga.Id == reader.GetInt32(0))
                            {
                                manga.Genres.Add(reader.GetString(1));
                            }
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

        public IActionResult OnPostMangaIdPage(int id)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToPage("/MangaId", new { mangaId = id });
            else
                return RedirectToPage("Authorization");
        }
    }
}
