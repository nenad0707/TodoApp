using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace TodoLibrary.DataAccess;

public class SqlDataAccess
{
  private readonly IConfiguration _config;

  public SqlDataAccess(IConfiguration config)
  {
    this._config = config;
  }

  public async Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
  {
    string connectionString = _config.GetConnectionString(connectionStringName)!;

    using IDbConnection connection = new SqlConnection(connectionString);

    var rows = await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

    return rows.ToList();
  }
}
