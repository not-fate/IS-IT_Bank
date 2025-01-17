using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HopeBank.Pages
{
    public class LoginModel : PageModel
    {
        public LoginModel(){

        }
        public string Error { get; set; } = "";
        public string Phone { get; set; } = "";
        public async Task OnGet()
        {
            await TryLogOut();
        }
        public async Task<IActionResult> OnPost(string phone, string email, string password,string firstname,string lastname, string isregister,[FromServices] DbCore db, string ReturnUrl = null)
        {
            await TryLogOut();

            var passhash = Utils.ToHash256(password);

            var userId = -1;
            if (!String.IsNullOrEmpty(isregister))
            {
                var amount = 0;
                var maxcredit = 0;
                var sql = @"begin tran
                   insert [account](phone,email,passhash,firstname,lastname)
                   values (@phone,@email,@passhash,@firstname,@lastname)
                   declare @id int=SCOPE_IDENTITY();
                   insert into balance(account_id,amount,maxcredit)
                   values (@id,@amount,@maxcredit)
                   commit tran
                   select @id";
                userId = await db.QueryFirstOrDefault<int>(sql,
                    new
                    {
                        phone,
                        email,
                        passhash,
                        firstname,
                        lastname,
                        amount,
                        maxcredit
                    });

            }

            var usr = userId>0?(await db.AccountById(userId)): await db.QueryFirstOrDefault<account?>("select * from [account] where phone=@phone or email=@email", new { phone, email });

            if (usr == null || usr.passhash != passhash)
            {
                Error = "Bad credental";
                Phone = phone;
                return Page();
            }
          
            var claims = new List<Claim>
            {
                 new (ClaimTypes.NameIdentifier,usr.id.ToString()),
                 new (ClaimTypes.MobilePhone, phone),
                 new (ClaimTypes.Email, email??String.Empty),
                //new Claim(ClaimTypes.Name, user.Username),  
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);
            ReturnUrl = ReturnUrl ?? "/";            
            return Redirect(ReturnUrl);
            //return new OkResult();
        }

        async Task TryLogOut()
        {
            if (this.HttpContext.User.Identity == null || !this.HttpContext.User.Identity.IsAuthenticated)
                return;
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
