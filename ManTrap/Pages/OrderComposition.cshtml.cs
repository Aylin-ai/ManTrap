using ManTrap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace ManTrap.Pages
{
    public class OrderCompositionModel : PageModel
    {
        [BindProperty(Name = "orderId", SupportsGet = true)]
        public int OrderId { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public OrderCompositionModel()
        {
        }

        public void OnGet()
        {
            GetOrderComposition();
        }

        public IActionResult OnPostMangaIdPage(int id)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToPage("/MangaId", new { mangaId = id });
            else
                return RedirectToPage("Authorization");
        }

        void GetOrderComposition()
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                string sql = "select contractcomposition.Manga_Id, manga.RussianName, " +
                    "contractcomposition.SourceOfChapterToTranslate, " +
                    "contractcomposition.ChapterId " +
                    "from contractcomposition inner join manga " +
                    "on contractcomposition.Manga_Id = manga.Id " +
                    "where contractcomposition.Contract_Id = @contractId;";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@contractId", OrderId);

                List<OrderItem> list = new List<OrderItem>();

                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        list.Add(new OrderItem()
                        {
                            MangaId = reader.GetInt32(0),
                            RussianName = reader.GetString(1),
                            SourceOfChapterToTranslate = reader.GetString(2),
                            ChapterId = reader.GetInt32(3),
                        });
                    }
                }
                OrderItems = list;
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
