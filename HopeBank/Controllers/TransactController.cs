using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HopeBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactController : ControllerBase
    {
        DbCore _db;
        public TransactController(DbCore db) {
            _db = db;
        }

        [HttpGet(nameof(list))]
        public async Task<object> list()
        {
            var userId = this.HttpContext.GetUserId();
            var trans = await _db.Query<transact>("select * from transact where account_from=@userId or account_to=@userId", new { userId });

            var usersIds = trans.Select(x => x.account_from).Concat(trans.Select(x => x.account_to)).ToHashSet();
            var users = await _db.Query<account>("select * from account where id in @ids", new { ids = usersIds });
            var userDic = users.ToDictionary(x => x.id);

            var lst = trans.Select(t => {
                var partherId = t.account_to == userId ? t.account_from : t.account_to;
                var parther=userDic.GetValueOrDefault(partherId);
                return new
                {
                    t.id,
                    type = t.account_to == userId ? trans_type.Income : trans_type.Outcome,
                    amount = t.amount * 0.01,
                    t.detail,
                    partner = $"{parther.firstname} {parther.lastname} ({parther.phone})",
                    date = Utils.DateFromTicks(t.id).ToString()
                };
            }).ToList();
            return lst;
        }

        [HttpPost(nameof(create_transact))]
        public async Task<object> create_transact([FromBody]create_transact_req req)
        {
            var amountInt = (int)(req.amount * 100);

            var userId = this.HttpContext.GetUserId();

            var balans = await _db.QueryFirstOrDefault<balance>("select * from  [balance] where account_id=@userId", new { userId });
            if (balans == null) return StatusCode(500);
            if (balans.amount+balans.maxcredit < amountInt) return BadRequest("Too low balance");
            var account_from = userId;

            var tx_id = Utils.NextTick();

            var error=await _db.QueryFirstOrDefault<int>(SQL.SqlScripts.create_transact,
                new
                {
                    amount= amountInt,
                    id = tx_id,
                    account_from = userId,
                    account_to = req.account_to,
                    detail= req.detail ?? string.Empty,
                    extern_account= req.extern_account ?? string.Empty
                });

            if (error == 0) return Ok(tx_id);

            return BadRequest("Too low balanse");
        }
    }
    public record create_transact_req(int account_to, decimal amount, string? detail = null, string? extern_account = null);
    public enum trans_type
    {
        None = 0,
        Income,
        Outcome
    }
}
