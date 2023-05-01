using ManTrap.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using Serilog;
using System.Collections.Generic;
using System.Security.Claims;

namespace ManTrap.Pages
{
    public class AddMangaModel : PageModel
    {
        public List<SelectListItem> Genres { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Authors { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Artists { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Publishers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TranslateTeams { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ReleaseFormats { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> Types { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TranslateStatuses { get; set; } = new List<SelectListItem>();

        public AddMangaModel()
        {
            Task.Run(GetReleaseFormats);
            Task.Run(GetTranslateTeams);
            Task.Run(GetData);
            Task.Run(GetAuthors);
        }

        public async void OnGetAsync()
        {
            await GetStatuses();
            await GetTranslateStatuses();
            await GetTypes();
            await GetArtists();
            await GetPublishers();
        }

        public async Task<IActionResult> OnPostUpload(IFormFile file, string originalName, string russianName,
            string englishName, int yearOfRelease,
            string[] authors, string[] artists, string[] publishers,
            string[] genres, string[] releaseFormats, string[] translateTeams,
            string status, string translateStatus, string sourceOfOriginal,
            string overview, string type = "1")
        {
            try
            {
                if (russianName == null || type == null
                    || authors == null || artists == null
                    || publishers == null || status == null
                    || overview == null)
                {
                    return BadRequest("Вы не ввели все данные");
                }
                else if (genres == null || releaseFormats == null)
                {
                    return BadRequest("Вы не ввели все данные");
                }
                else
                {
                    MySqlConnection conn = DBUtils.GetDBConnection();
                    conn.Open();
                    try
                    {
                        string sql = "insert into manga " +
                            "(manga.OriginalName, " +
                            "manga.RussianName, manga.EnglishName, " +
                            "manga.MangaType_Id, manga.MangaStatus_Id, " +
                            "TranslateStatus_Id, YearOfRelease, " +
                            "SourceOfOriginal, Overview, Checked) " +
                            "values " +
                            "(@originalName, @russianName, @englishName, " +
                            "@mangaType, " +
                            "@mangaStatus, " +
                            "@translateStatus, " +
                            "@yearOfRelease, @sourceOfOriginal, @overview, 0)";

                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = sql;
                        cmd.Connection = conn;

                        cmd.Parameters.AddWithValue("@originalName", originalName);
                        cmd.Parameters.AddWithValue("@russianName", russianName);
                        cmd.Parameters.AddWithValue("@englishName", englishName);
                        cmd.Parameters.AddWithValue("@mangaType", type);
                        cmd.Parameters.AddWithValue("@mangaStatus", status);
                        cmd.Parameters.AddWithValue("@translateStatus", translateStatus);
                        cmd.Parameters.AddWithValue("@yearOfRelease", yearOfRelease);
                        cmd.Parameters.AddWithValue("@sourceOfOriginal", sourceOfOriginal);
                        cmd.Parameters.AddWithValue("@overview", overview);

                        await cmd.ExecuteNonQueryAsync();


                        sql = "select Id from manga where " +
                            "OriginalName = @originalName and " +
                            "RussianName = @russianName and " +
                            "EnglishName = @englishName";
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
                        cmd.Parameters.Clear();

                        if (authors != null)
                        {
                            sql = "insert into mangaauthors " +
                                "values (@mangaId, @authorId);";
                            cmd.CommandText = sql;

                            for (int i = 0; i < authors.Length; i++)
                            {
                                cmd.Parameters.AddWithValue("@mangaId", Id);
                                cmd.Parameters.AddWithValue("authorId", authors[i]);
                                await cmd.ExecuteNonQueryAsync();
                                cmd.Parameters.Clear();
                            }
                        }

                        if (artists  != null)
                        {
                            sql = "insert into mangaartists " +
                                "values (@mangaId, @artistId);";
                            cmd.CommandText = sql;

                            for (int i = 0; i < artists.Length; i++)
                            {
                                cmd.Parameters.AddWithValue("@mangaId", Id);
                                cmd.Parameters.AddWithValue("artistId", artists[i]);
                                await cmd.ExecuteNonQueryAsync();
                                cmd.Parameters.Clear();
                            }
                        }

                        if (genres != null)
                        {
                            sql = "insert into mangagenres " +
                                "values (@mangaId, @genreId);";
                            cmd.CommandText = sql;

                            for (int i = 0; i < genres.Length; i++)
                            {
                                cmd.Parameters.AddWithValue("@mangaId", Id);
                                cmd.Parameters.AddWithValue("genreId", genres[i]);
                                await cmd.ExecuteNonQueryAsync();
                                cmd.Parameters.Clear();
                            }
                        }

                        if (publishers != null)
                        {
                            sql = "insert into mangapublishers " +
                                "values (@mangaId, @publisherId);";
                            cmd.CommandText = sql;

                            for (int i = 0; i < publishers.Length; i++)
                            {
                                cmd.Parameters.AddWithValue("@mangaId", Id);
                                cmd.Parameters.AddWithValue("publisherId", publishers[i]);
                                await cmd.ExecuteNonQueryAsync();
                                cmd.Parameters.Clear();
                            }
                        }

                        if (releaseFormats != null)
                        {
                            sql = "insert into mangareleaseformats " +
                                "values (@mangaId, @releaseFormatId);";
                            cmd.CommandText = sql;

                            for (int i = 0; i < releaseFormats.Length; i++)
                            {
                                cmd.Parameters.AddWithValue("@mangaId", Id);
                                cmd.Parameters.AddWithValue("releaseFormatId", releaseFormats[i]);
                                await cmd.ExecuteNonQueryAsync();
                                cmd.Parameters.Clear();
                            }
                        }

                        if (translateTeams != null)
                        {
                            sql = "insert into mangatranslateteams " +
                                "values (@mangaId, @translateTeamId);";
                            cmd.CommandText = sql;

                            for (int i = 0; i < translateTeams.Length; i++)
                            {
                                cmd.Parameters.AddWithValue("@mangaId", Id);
                                cmd.Parameters.AddWithValue("translateTeamId", translateTeams[i]);
                                await cmd.ExecuteNonQueryAsync();
                                cmd.Parameters.Clear();
                            }
                        }

                        return RedirectToPage("/AddManga");

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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Page();
            }
        }

        private async Task GetData()
        {
            Genres.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "select * from genre";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        Genres.Add(new SelectListItem()
                        {
                            Value = reader.GetInt32(0).ToString(),
                            Text = reader.GetString(1)
                        });
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

        private async Task GetAuthors()
        {
            Authors.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "select * from author";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                List<SelectListItem> authors = new List<SelectListItem>();

                var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        authors.Add(new SelectListItem()
                        {
                            Value = reader.GetInt32(0).ToString(),
                            Text = reader.GetString(1)
                        });
                    }
                    Authors = authors;
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

        private async Task GetArtists()
        {
            Artists.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "select * from artist";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                List<SelectListItem> list = new List<SelectListItem>();

                var reader = await cmd.ExecuteReaderAsync();
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
                    Artists = list;
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

        private async Task GetPublishers()
        {
            Publishers.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "select * from publisher";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                List<SelectListItem> list = new List<SelectListItem>();

                var reader = await cmd.ExecuteReaderAsync();
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
                    Publishers = list;
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

        private async Task GetTranslateTeams()
        {
            TranslateTeams.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "select * from translateteam";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                List<SelectListItem> list = new List<SelectListItem>();

                var reader = await cmd.ExecuteReaderAsync();
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
                    TranslateTeams = list;
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

        private async Task GetReleaseFormats()
        {
            ReleaseFormats.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "select * from releaseformat";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                List<SelectListItem> list = new List<SelectListItem>();

                var reader = await cmd.ExecuteReaderAsync();
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
                    ReleaseFormats = list;
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

        private async Task GetTypes()
        {
            Types.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "select * from mangatype";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                List<SelectListItem> list = new List<SelectListItem>();

                var reader = await cmd.ExecuteReaderAsync();
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
                    Types = list;
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

        private async Task GetStatuses()
        {
            Statuses.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "select * from mangastatus";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                List<SelectListItem> list = new List<SelectListItem>();

                var reader = await cmd.ExecuteReaderAsync();
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

        private async Task GetTranslateStatuses()
        {
            TranslateStatuses.Clear();
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "select * from translatestatus";

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;

                List<SelectListItem> list = new List<SelectListItem>();

                var reader = await cmd.ExecuteReaderAsync();
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
                    TranslateStatuses = list;
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
