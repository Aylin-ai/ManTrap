using ManTrap.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Security.Claims;

namespace ManTrap.Pages
{
    public class AuthorizationModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public string ErrorMessage { get; set; }
        public string UserImageSrc { get; set; }
        private int _userRole;

        public AuthorizationModel(ILogger<IndexModel> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostRegistration(string Login, string Password1, string Password2)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (Login == null || Password1 == null || Password2 == null)
            {
                ErrorMessage = "Вы ввели не все данные";
                return Page();
            }
            else
            {
                if (Password1 != Password2)
                {
                    ErrorMessage = "Пароли не совпадают";
                    return Page();
                }
                MySqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                try
                {
                    string sql = "select * from userinformation where " +
                        "Login = @login";

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = sql;
                    cmd.Connection = conn;

                    cmd.Parameters.AddWithValue("@login", Login);

                    var reader = cmd.ExecuteReader();
                    cmd.Parameters.Clear();

                    if (reader.HasRows)
                    {
                        ErrorMessage = "Пользователь с таким логином уже существует";
                        return Page();
                    }
                    else
                    {
                        await reader.CloseAsync();
                        sql = "insert into userinformation " +
                        "(Login, Pasword, RoleInformation_Id) " +
                        "values (@login, @password, 1);";

                        cmd = new MySqlCommand();
                        cmd.CommandText = sql;
                        cmd.Connection = conn;

                        cmd.Parameters.AddWithValue("@login", Login);
                        cmd.Parameters.AddWithValue("@password", Password1);

                        await cmd.ExecuteNonQueryAsync();

                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, $"{Login}"),
                        new Claim(ClaimTypes.Role, "User")
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

                        return RedirectToPage("/UserProfile", new { login = Login });
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    return Page();
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public async Task<IActionResult> OnPostAuthorization(string Login, string Password1)
        {
            if (!ModelState.IsValid)
                return Page();
            if (Login == null || Password1 == null)
            {
                ErrorMessage = "Вы ввели не все данные";
                return Page();
            }
            else
            {
                MySqlConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                try
                {
                    string sql = "select * from userinformation where " +
                        "Login = @login and Pasword = @password";

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = sql;
                    cmd.Connection = conn;

                    cmd.Parameters.AddWithValue("@login", Login);
                    cmd.Parameters.AddWithValue("@password", Password1);

                    var reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        ErrorMessage = "Неправильный логин или пароль";
                        return Page();
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            _userRole = reader.GetInt32(4);
                        }

                        var claims = new List<Claim>();
                        if (_userRole == 1)
                        {
                            claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, $"{Login}"),
                            new Claim(ClaimTypes.Role, "User")
                        };
                        }
                        else if (_userRole == 2)
                        {
                            claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, $"{Login}"),
                            new Claim(ClaimTypes.Role, "Translator")
                        };
                        }

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
                        return RedirectToPage("/UserProfile");
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.InnerException.ToString();
                    return Page();
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
    }
}
