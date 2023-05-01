using Google.Protobuf;
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

        public List<SelectListItem> Kinds { get; set; } = new List<SelectListItem>();
        public int Kind { get; set; }

        public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
        public int Status { get; set; }

        public List<SelectListItem> Genres { get; set; } = new List<SelectListItem>();
        public int Genre { get; set; }

        public string Search { get; set; }

        public IndexModel()
        {
            GetGenres();
            GetStatuses();
            GetTypes();
            GetMangas();
        }

        public void OnGet()
        {
        }

        public void OnPostById(int genreId, int statusId, int typeId, string search)
        {
            if (search != null)
            {
                Search = search;
                GetMangas(search);
            }
            else
            {
                Genre = genreId;
                Status = statusId;
                Kind = typeId;

                GetMangas(Genre, Status, Kind);
            }
        }

        public IActionResult OnPostMangaIdPage(int id)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToPage("/MangaId", new { mangaId = id });
            else
                return RedirectToPage("Authorization");
        }

        private void GetMangas()
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

                var reader = cmd.ExecuteReader();
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
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();
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

        private void GetMangas(int genreId, int statusId, int typeId)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                string sql = "";

                if (genreId == 0 && statusId == 0 && typeId == 0)
                {
                    sql = "select manga.Id, manga.ImageSource, manga.RussianName, manga.EnglishName, " +
                        "mangatype.Title, mangastatus.Title " +
                        "from manga inner join mangatype " +
                        "on manga.MangaType_Id = mangatype.Id " +
                        "inner join mangastatus " +
                        "on manga.MangaStatus_Id = mangastatus.Id";
                }
                else
                {
                    if (genreId == 0 && statusId == 0)
                    {
                        sql = "select manga.Id, manga.ImageSource, manga.RussianName, manga.EnglishName, " +
                            "mangatype.Title, mangastatus.Title " +
                            "from manga inner join mangatype " +
                            "on manga.MangaType_Id = mangatype.Id " +
                            "inner join mangastatus " +
                            "on manga.MangaStatus_Id = mangastatus.Id " +
                            "where manga.MangaType_Id = @typeId;";
                    }
                    else if (genreId == 0 && typeId == 0)
                    {
                        sql = "select manga.Id, manga.ImageSource, manga.RussianName, manga.EnglishName, " +
                            "mangatype.Title, mangastatus.Title " +
                            "from manga inner join mangatype " +
                            "on manga.MangaType_Id = mangatype.Id " +
                            "inner join mangastatus " +
                            "on manga.MangaStatus_Id = mangastatus.Id " +
                            "where " +
                            "manga.MangaStatus_Id = @statusId;";
                    }
                    else if (typeId == 0 && statusId == 0)
                    {
                        sql = "select manga.Id, manga.ImageSource, manga.RussianName, manga.EnglishName, " +
                            "mangatype.Title, mangastatus.Title " +
                            "from manga inner join mangatype " +
                            "on manga.MangaType_Id = mangatype.Id " +
                            "inner join mangastatus " +
                            "on manga.MangaStatus_Id = mangastatus.Id " +
                            "inner join mangagenres " +
                            "on manga.Id = mangagenres.Manga_Id " +
                            "where mangagenres.Genre_Id = @genreId;";
                    }
                    else if (genreId == 0)
                    {
                        sql = "select manga.Id, manga.ImageSource, manga.RussianName, manga.EnglishName, " +
                            "mangatype.Title, mangastatus.Title " +
                            "from manga inner join mangatype " +
                            "on manga.MangaType_Id = mangatype.Id " +
                            "inner join mangastatus " +
                            "on manga.MangaStatus_Id = mangastatus.Id " +
                            "where manga.MangaType_Id = @typeId and " +
                            "manga.MangaStatus_Id = @statusId;";
                    }
                    else if (statusId == 0)
                    {
                        sql = "select manga.Id, manga.ImageSource, manga.RussianName, manga.EnglishName, " +
                            "mangatype.Title, mangastatus.Title " +
                            "from manga inner join mangatype " +
                            "on manga.MangaType_Id = mangatype.Id " +
                            "inner join mangastatus " +
                            "on manga.MangaStatus_Id = mangastatus.Id " +
                            "inner join mangagenres " +
                            "on manga.Id = mangagenres.Manga_Id " +
                            "where mangagenres.Genre_Id = @genreId and " +
                            "manga.MangaType_Id = @typeId;";
                    }
                    else if (typeId == 0)
                    {
                        sql = "select manga.Id, manga.ImageSource, manga.RussianName, manga.EnglishName, " +
                            "mangatype.Title, mangastatus.Title " +
                            "from manga inner join mangatype " +
                            "on manga.MangaType_Id = mangatype.Id " +
                            "inner join mangastatus " +
                            "on manga.MangaStatus_Id = mangastatus.Id " +
                            "inner join mangagenres " +
                            "on manga.Id = mangagenres.Manga_Id " +
                            "where mangagenres.Genre_Id = @genreId and " +
                            "manga.MangaStatus_Id = @statusId;";
                    }
                }

                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@typeId", typeId);
                cmd.Parameters.AddWithValue("@statusId", statusId);
                cmd.Parameters.AddWithValue("@genreId", genreId);

                List<Manga> list = new List<Manga>();

                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        list.Add(
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
                Mangas = list;
                reader.Close();

                sql = "select mangagenres.Manga_Id, genre.Title from mangagenres " +
                    "inner join genre on mangagenres.Genre_Id = genre.Id " +
                    "inner join manga on manga.Id = mangagenres.Manga_Id " +
                    "where manga.Id = @mangaId;";
                cmd.CommandText = sql;

                foreach (var manga in Mangas)
                {
                    cmd.Parameters.AddWithValue("@mangaId", manga.Id);
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            manga.Genres.Add(reader.GetString(1));
                        }
                    }
                    cmd.Parameters.Clear();
                    reader.Close();
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

        private void GetMangas(string search)
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
                    "on manga.MangaStatus_Id = mangastatus.Id " +
                    "inner join mangagenres " +
                    "on manga.Id = mangagenres.Manga_Id " +
                    $"where manga.RussianName like '%{search}%' or " +
                    $"manga.OriginalName like '%@{search}%' or " +
                    $"manga.EnglishName like '%{search}%' " +
                    "group by manga.Id;";

                cmd.CommandText = sql;

                List<Manga> list = new List<Manga>();

                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        list.Add(
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
                Mangas = list;
                reader.Close();

                sql = "select mangagenres.Manga_Id, genre.Title from mangagenres " +
                    "inner join genre on mangagenres.Genre_Id = genre.Id " +
                    "inner join manga on manga.Id = mangagenres.Manga_Id " +
                    "where manga.Id = @mangaId;";
                cmd.CommandText = sql;

                foreach (var manga in Mangas)
                {
                    cmd.Parameters.AddWithValue("@mangaId", manga.Id);
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            manga.Genres.Add(reader.GetString(1));
                        }
                    }
                    cmd.Parameters.Clear();
                    reader.Close();
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

        public void GetGenres()
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "select * from genre";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                var reader = cmd.ExecuteReader();
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Âñ¸"
                });
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = reader.GetInt32(0).ToString(),
                            Text = reader.GetString(1)
                        });
                    }
                    Genres = list;
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

        private void GetTypes()
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "select * from mangatype";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Âñ¸"
                });
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = reader.GetInt32(0).ToString(),
                            Text = reader.GetString(1)
                        });
                    }
                    Kinds = list;
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

        private void GetStatuses()
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "select * from mangastatus";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Âñ¸"
                });
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = reader.GetInt32(0).ToString(),
                            Text = reader.GetString(1)
                        });
                    }
                    Statuses = list;
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
