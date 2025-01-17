using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HopeBank.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public string UserName {get;set;}

        public async Task OnGet([FromServices]DbCore db)
        {
            //HttpContext.User.Identity.IsAuthenticated;
            //var userId = HttpContext.GetUserId();
            //var usr = await db.AccountById(userId);

            //UserName = $"{usr.firstname} {usr.lastname}";
        }
    }
}
