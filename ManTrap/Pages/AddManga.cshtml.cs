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
        }

        public async void OnGet()
        {
            await GetData();
            await GetAuthors();
            await GetArtists();
            await GetPublishers();
            await GetReleaseFormats();
            await GetStatuses();
            await GetTranslateStatuses();
            await GetTranslateTeams();
            await GetTypes();
        }

        public async Task<IActionResult> OnPostUpload(IFormFile file, string originalName, string russianName,
            string englishName, string type, int yearOfRelease,
            string author, string artist, string publisher,
            string[] genres, string[] releaseformats, string translateTeam,
            string status, string translateStatus, string sourceOfOriginal,
            string overview)
        {
            try
            {
                if (russianName == null || type == null
                    || author == null || artist == null
                    || publisher == null || status == null
                    || overview == null)
                {
                    return BadRequest("Вы не ввели все данные");
                }
                else if (genres == null || releaseformats == null)
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
                            "(select Id from mangatype where Title = @mangaType), " +
                            "(select Id from mangastatus where Title = @mangaStatus), " +
                            "(select Id from translatestatus where Title = @translateStatus), " +
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Page();
            }
        }

        private async Task GetData()
        {
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
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                string sql = "select * from author";

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
