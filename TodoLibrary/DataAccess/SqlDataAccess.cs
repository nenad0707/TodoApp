using Microsoft.Extensions.Configuration;

namespace TodoLibrary.DataAccess;

public class SqlDataAccess
{
  private readonly IConfiguration _config;

  public SqlDataAccess(IConfiguration config)
  {
    this._config = config;
  }
}
