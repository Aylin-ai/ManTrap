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
                    "mangatype.Title, mangastatus.Title, translatestatus.Title, " +
                    "manga.YearOfRelease, manga.Overview, manga.OriginalName " +
                    "from manga inner join mangatype " +
                    "on manga.MangaType_Id = mangatype.Id " +
                    "inner join mangastatus " +
                    "on manga.MangaStatus_Id = mangastatus.Id " +
                    "inner join translatestatus " +
                    "on manga.TranslateStatus_Id = translatestatus.Id " + 
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
                            OriginalName = reader.GetString(9),
                        };
                    }
                }
                reader.Close();

                sql = "select mangagenres.Manga_Id, genre.Title " +
                    "from mangagenres inner join genre " +
                    "on mangagenres.Genre_Id = genre.Id " +
                    "where Manga_Id = @id";
                cmd.CommandText = sql;

                reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Manga.Genres.Add(reader.GetString(1));
                    }
                }
                reader.Close();

                sql = "select mangaauthors.Manga_Id, author.Title " +
                    "from mangaauthors inner join author " +
                    "on mangaauthors.Author_Id = author.Id " +
                    "where Manga_Id = @id";
                cmd.CommandText = sql;
                reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Manga.Authors.Add(reader.GetString(1));
                    }
                }
                reader.Close();

                sql = "select mangaartists.Manga_Id, artist.Title " +
                    "from mangaartists inner join artist " +
                    "on mangaartists.Artist_Id = artist.Id " +
                    "where Manga_Id = @id";
                cmd.CommandText = sql;
                reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Manga.Artsits.Add(reader.GetString(1));
                    }
                }
                reader.Close();

                sql = "select mangapublishers.Manga_Id, publisher.Title " +
                    "from mangapublishers inner join publisher " +
                    "on mangapublishers.Publisher_Id = publisher.Id " +
                    "where Manga_Id = @id";
                cmd.CommandText = sql;
                reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Manga.Publishers.Add(reader.GetString(1));
                    }
                }
                reader.Close();

                sql = "select mangareleaseformats.Manga_Id, releaseformat.Title " +
                    "from mangareleaseformats inner join releaseformat " +
                    "on mangareleaseformats.ReleaseFormat_Id = releaseformat.Id " +
                    "where Manga_Id = @id";
                cmd.CommandText = sql;
                reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Manga.ReleaseFormats.Add(reader.GetString(1));
                    }
                }
                reader.Close();

                sql = "select mangatranslateteams.Manga_Id, translateteam.Title " +
                    "from mangatranslateteams inner join translateteam " +
                    "on mangatranslateteams.TranslateTeam_Id = translateteam.Id " +
                    "where Manga_Id = @id";
                cmd.CommandText = sql;
                reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Manga.ReleaseFormats.Add(reader.GetString(1));
                    }
                }
                reader.Close();

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
