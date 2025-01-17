using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public static class Utils
{
    public static string ToHash256(string code)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(code);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
    public static int GetUserId(this HttpContext context)
    {
        var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        if (string.IsNullOrWhiteSpace(userIdStr)) return 0;
        return int.TryParse(userIdStr, out var _r) ? _r : 0;
    }

    static long _lastTick = 0;
    static object _lockObj = new();
    public static long NextTick()
    {
        var tick = (DateTime.UtcNow - DateTime.UnixEpoch).Ticks / 10;
        lock (_lockObj)
        {
            if (tick <= _lastTick + 10) tick = _lastTick + 10;
            _lastTick = tick;
        }
        return tick;
    }
    public static DateTime DateFromTicks(long id) => DateTime.UnixEpoch.AddTicks(id * 10);
}



