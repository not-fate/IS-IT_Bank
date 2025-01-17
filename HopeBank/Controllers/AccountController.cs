using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HopeBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        DbCore _db;
        public AccountController(DbCore db) => _db = db;

        [HttpGet("search")]
        public async Task<object> search(string? phone = null)
        {
            var acc = await _db.AccountByIdOrPhone(-1, phone!);
            if (acc == null) return NotFound();
            return new {
                id = acc.id,
                name = $"{acc.firstname} {acc.lastname}"
            };
        }

        [HttpGet(nameof(current_user))]
        public async Task<object> current_user()
        {
            var userId = this.HttpContext.GetUserId();

            var usr = await _db.AccountById(userId);
            var balance = await _db.BalanceByAccId(userId);
            if (usr == null || balance == null) return NotFound();
            return new
            {
                usr.id,
                usr.phone,
                usr.firstname,
                usr.lastname,
                usr.email,
                isadmin= usr.isAdmin,
                amount =balance.amount*0.01m,
                maxcredit= balance.maxcredit*0.01m
            };
        }

        [HttpGet(nameof(allusers))]
        public async Task<object> allusers()
        {
            var userId = this.HttpContext.GetUserId();

            var usr = await _db.AccountById(userId);
            if (usr == null) return NotFound();

            var all = await _db.Query<account>("select * from account");
            var balances = await _db.Query<balance>("select * from balance");
            var balansDic = balances.ToDictionary(x => x.account_id);

            var rez = all.Select(x =>
            {
                var balance = balansDic[x.id];
                return new
                {
                    x.id,
                    x.phone,
                    x.firstname,
                    x.lastname,
                    x.email,
                    amount = balance.amount * 0.01m,
                    maxcredit =balance.maxcredit * 0.01m,
                    isadmin = x.isAdmin
                };
            }).ToList();
            return rez;
        }

        [HttpPost(nameof(edit))]
        public async Task<object> edit([FromBody]edit_req req) {           
            var userId = this.HttpContext.GetUserId();
            await _db.Execute("update account set isadmin=isnull(@isadmin,isadmin) where id=@id", req);
            await _db.Execute("update balance set maxcredit=isnull(@maxcredit,maxcredit) where account_id=@id", new { 
                id=req.id, 
                maxcredit= req.maxcredit.HasValue? (int)req.maxcredit*100:(int?)null
            });           
            return Ok();
        }

        public record edit_req(int id,string? phone=null,string? email=null,string? passhash=null,string ?firstname=null,string? lastname=null,bool? isadmin=null,double? maxcredit=null);
    }
}
