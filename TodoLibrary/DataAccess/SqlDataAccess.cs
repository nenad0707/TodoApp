using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace TodoLibrary.DataAccess;

public class SqlDataAccess
{

  private readonly IConfiguration _config;

  /// <summary>
  /// Initializes a new instance of the <see cref="SqlDataAccess"/> class.
  /// </summary>
  /// <param name="config">The configuration object.</param>
  public SqlDataAccess(IConfiguration config)
  {
    this._config = config;
  }

  /// <summary>
  /// Loads data from the database using a stored procedure.
  /// </summary>
  /// <typeparam name="T">The type of data to load.</typeparam>
  /// <typeparam name="U">The type of parameters for the stored procedure.</typeparam>
  /// <param name="storedProcedure">The name of the stored procedure.</param>
  /// <param name="parameters">The parameters for the stored procedure.</param>
  /// <param name="connectionStringName">The name of the connection string in the configuration.</param>
  /// <returns>A list of loaded data.</returns>
  public async Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
  {
    string connectionString = _config.GetConnectionString(connectionStringName)!;

    using IDbConnection connection = new SqlConnection(connectionString);

    var rows = await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

    return rows.ToList();
  }

  public Task SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
  {
    string connectionString = _config.GetConnectionString(connectionStringName)!;

    using IDbConnection connection = new SqlConnection(connectionString);

    return connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
  }
}

