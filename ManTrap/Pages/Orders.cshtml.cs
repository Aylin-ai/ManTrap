using ManTrap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace ManTrap.Pages
{
    public class OrdersModel : PageModel
    {
        public List<Order> Orders { get; set; } = new List<Order>();

        public OrdersModel()
        {
            GetOrders();
        }

        public void OnGet()
        {
        }

        public IActionResult OnPostOrderComposition(int id)
        {
            return RedirectToPage("/OrderComposition", new { orderId = id });
        }

        public IActionResult OnPostTakeOrder(int id)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                string sql = "select translateteamcomposition.TranslateTeam_Id, translateteam.Title from " +
                    "translateteam inner join translateteamcomposition " +
                    "on translateteamcomposition.TranslateTeam_Id = translateteam.Id " +
                    "where translateteamcomposition.UserInformation_Login = @user";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("user", User.Identity.Name);

                int teamId = 0;
                string teamName = "";

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        teamId = reader.GetInt32(0);
                        teamName = reader.GetString(1);
                    }
                }
                else
                {
                    return BadRequest("ƒл€ прин€ти€ заказа вы должны быть в команде. —оздайте свою команду либо присоединитесь к другой.");
                }
                reader.Close();

                sql = "update contract " +
                    "set TranslateTeam_Id = @teamId, " +
                    "ContractStatus_Id = 2 " +
                    "where Id = @contractId";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@teamId", teamId);
                cmd.Parameters.AddWithValue("@contractId", id);
                cmd.ExecuteNonQuery();
                return RedirectToPage("/Index");
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

        public void GetOrders()
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                string sql = "select contract.Id, contract.DateOfCreation, " +
                    "contractstatus.Title, contract.UserInformation_Login_Customer, " +
                    "translateteam.Title, contract.Cost, contract.TargetDate, " +
                    "contract.Paid from contract " +
                    "inner join contractstatus " +
                    "on contract.ContractStatus_Id = contractstatus.Id " +
                    "inner join translateteam " +
                    "on translateteam.Id = contract.TranslateTeam_Id " +
                    "where contract.TranslateTeam_Id = 1;";
                cmd.CommandText = sql;

                List<Order> list = new List<Order>();

                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        list.Add(new Order
                        {
                            Id = reader.GetInt32(0),
                            DateOfCreation = reader.GetDateTime(1).ToString("yyyy-MM-dd"),
                            OrderStatus = reader.GetString(2),
                            Customer = reader.GetString(3),
                            TransalteTeam = reader.GetString(4),
                            Cost = reader.GetFloat(5),
                            TargetDate = reader.GetDateTime(6).ToString("yyyy-MM-dd"),
                            IsPaid = reader.GetBoolean(7),
                        });
                    }
                }
                Orders = list;
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
