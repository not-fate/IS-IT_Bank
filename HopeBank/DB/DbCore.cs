using Dapper;
using Microsoft.Data.SqlClient;

public class DbCore
{
    string _connectionStr;
    public DbCore(string connectionStr) => _connectionStr = connectionStr;

    public async Task<int> Execute(string sql, object? param = null, int? commandTimeout = null)
    {
        using var conn = new SqlConnection(_connectionStr);
        return await conn.ExecuteAsync(sql, param, commandTimeout: commandTimeout);
    }
    public async Task<List<T>> Query<T>(string sql, object? param = null, int? commandTimeout = null)
    {
        using var conn = new SqlConnection(_connectionStr);
        var rez = await conn.QueryAsync<T>(sql, param, commandTimeout: commandTimeout);
        return rez.ToList();
    }
    public async Task<T?> QueryFirstOrDefault<T>(string sql, object? param = null, int? commandTimeout = null)
    {
        using var conn = new SqlConnection(_connectionStr);
        return await conn.QueryFirstOrDefaultAsync<T>(sql, param, commandTimeout: commandTimeout);
    }

    public void InitMigration()
    {
        var connBuild = new SqlConnectionStringBuilder(_connectionStr);
        var dbName = connBuild.InitialCatalog;
        connBuild.InitialCatalog = "master";
        using var conn = new SqlConnection(connBuild.ConnectionString);
        var existed = conn.Query<string>($"SELECT [name] FROM sys.databases WHERE name = '{dbName}'").ToList();
        if (existed.Count > 0) return;
        conn.Execute(@$"IF db_id('{dbName}') IS NULL 
                    CREATE DATABASE [{dbName}];");

        using var dbconn = new SqlConnection(_connectionStr);
        dbconn.Execute(HopeBank.SQL.SqlScripts.FillDb);
    }

    public async Task<account?> CreateAccount(string phone, string email, string passhash, string firstname, string lastname)
    {

        return await QueryFirstOrDefault<account>(@"insert into [dbo].[account]([phone],[email],[passhash],[firstname],[lastname]) 
            OUTPUT inserted.* values (@phone,@email,@passhash,@firstname,@lastname", new { phone, email, passhash, firstname, lastname });
    }
    public Task<account?> AccountById(int id) => QueryFirstOrDefault<account>("select * from [account] where id=@id", new { id });
    public Task<account?> AccountByIdOrPhone(int id, string phone) => QueryFirstOrDefault<account>("select * from [account] where id=@id or phone=@phone", new { id, phone });
    public Task<balance?> BalanceByAccId(int id) => QueryFirstOrDefault<balance>("select * from balance where account_id=@id", new { id });
    // public 
}

public class account
{
    public int id { get; set; }
    public string phone { get; set; }
    public string email { get; set; }
    public string passhash { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public bool isAdmin { get; set; }
    public bool isExternalApi { get; set; }
}

public class balance
{
    public int account_id { get; set; }
    public long amount { get; set; }
    public long maxcredit { get; set; }
}
public enum transact_type
{
    None,
    Inner = 0
}
public class transact
{
    public long id { get; set; }
    public int account_from { get; set; }
    public int account_to { get; set; }
    public long amount { get; set; }
    public string detail { get; set; }
}

