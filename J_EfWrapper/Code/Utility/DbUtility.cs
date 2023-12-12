using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace J_EfWrapper.Code.Utility
{
  public static class DbUtility
  {
    public static SqlConnection GetConnection(DbConnection DbConnection)
    {
      return new SqlConnection(DbConnection.ConnectionString);
    }
  }
}
