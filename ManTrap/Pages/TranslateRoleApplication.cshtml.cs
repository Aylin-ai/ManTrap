using ManTrap.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pkcs;
using System.Security.Claims;

namespace ManTrap.Pages
{
    public class TranslateRoleApplicationModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        [BindProperty(Name = "login", SupportsGet = true)]
        public string Login { get; set; }

        public bool IsApplicationAlreadyExists { get; set; }
        public bool IsApplicationAccepted { get; set; }

        public TranslateRoleApplicationModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnGet()
        {
            CheckUsersApplicationExistance();
        }

        public IActionResult OnPostAddApplication(string exampleWork)
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                string sql = "insert into translatorapplication " +
                    "values (@login, @date, @ExampleWork, 0)";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@login", User.Identity.Name);
                cmd.Parameters.AddWithValue("@date", DateTime.Today.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("ExampleWork", exampleWork);
                cmd.ExecuteNonQuery();
                return RedirectToPage("/UserProfile", new { message = "Ваша заявка подана. Ожидайте одобрения или отказа" });
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

        public async Task<IActionResult> OnPostAcceptTranslateRoleAsync()
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                string sql = "update userinformation set " +
                    "RoleInformation_Id = 2 " +
                    "where Login = @login";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@login", User.Identity.Name);
                cmd.ExecuteNonQuery();

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, User.Identity.Name),
                        new Claim(ClaimTypes.Role, "Translator")
                    };

                var identity = new ClaimsIdentity(
                    claims, "MyCookieAuthenticationScheme");

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1)
                };

                var principal = new ClaimsPrincipal(identity);

                await _httpContextAccessor.HttpContext.SignInAsync(
                    "MyCookieAuthenticationScheme",
                    principal,
                    authProperties);

                return RedirectToPage("/UserProfile", new { message = "Поздравляю, теперь вы переводчик" });
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

        public void CheckUsersApplicationExistance()
        {
            MySqlConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                string sql = "select * from translatorapplication where UserInformation_Login = @login";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@login", User.Identity.Name);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    IsApplicationAlreadyExists = true;
                    while (reader.Read())
                    {
                        IsApplicationAccepted = reader.GetBoolean(3);
                    }
                }
                else
                {
                    IsApplicationAlreadyExists = false;
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
