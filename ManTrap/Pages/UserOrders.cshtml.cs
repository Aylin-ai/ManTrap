using ManTrap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace ManTrap.Pages
{
    public class UserOrdersModel : PageModel
    {
        public List<Order> Orders { get; set; } = new List<Order>();

        public void OnGet()
        {
            GetOrders();
        }

        public IActionResult OnPostOrderComposition(int id)
        {
            return RedirectToPage("/OrderComposition", new { orderId = id });
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
                    "where contract.UserInformation_Login_Customer = @customerLogin;";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@customerLogin", User.Identity.Name);

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
