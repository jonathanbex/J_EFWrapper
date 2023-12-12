using System.Data.Common;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Dapper;
using Microsoft.Data.SqlClient;
using J_EfWrapper.Code.Utility;

namespace J_EfWrapper.Code
{
  public static class J_EfExtensionMethods
  {

    public static IEnumerable<dynamic> DynamicListFromSql(this DbContext db, string Sql, Dictionary<string, object> Params)
    {
      using (var cmd = db.Database.GetDbConnection().CreateCommand())
      {
        cmd.CommandText = Sql;
        if (cmd.Connection.State != ConnectionState.Open) { cmd.Connection.Open(); }

        foreach (KeyValuePair<string, object> p in Params)
        {
          DbParameter dbParameter = cmd.CreateParameter();
          dbParameter.ParameterName = p.Key;
          dbParameter.Value = p.Value;
          cmd.Parameters.Add(dbParameter);
        }

        using (var dataReader = cmd.ExecuteReader())
        {
          while (dataReader.Read())
          {
            var row = new ExpandoObject() as IDictionary<string, object>;
            for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
            {
              row.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
            }
            yield return row;
          }
        }
      }
    }
    public static string SqlFormat(string str, params object[] args)
    {
      int count = args.Length;

      for (int i = 0; i < count; i++)
      {
        if (args[i] == null)
        {
          args[i] = "NULL";
          continue;
        }
        if (args[i] == DBNull.Value)
        {
          args[i] = "NULL";
          continue;
        }
        if (args[i].GetType() == typeof(string))
          args[i] = "'" + args[i] + "'";
        if (args[i].GetType() == typeof(DateTime))
          args[i] = "'" + args[i] + "'";
        if (args[i].GetType() == typeof(bool))
          args[i] = ((bool)args[i]) ? "1" : "0";
      }

      return string.Format(str, args);
    }

    /// <summary>
    /// use this when casting sql to models not in context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Database"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<T>> ExecuteSqlRawWithDapperAsync<T>(this DatabaseFacade Database, string sql)
    {
      // if (sql.Contains("@")) return await ExecuteSqRawlWithParameters<T>(Database, sql, parameters);
      using (var _db = DbUtility.GetConnection(Database.GetDbConnection()) as IDbConnection)
      {
        _db.Open();

        var sqlParamateredString = sql;

        return await _db.QueryAsync<T>(sqlParamateredString);

      }

    }

    /// <summary>
    /// use this when casting sql to models not in context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Database"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<T>> ExecuteSqlRawWithDapperAsync<T>(this DatabaseFacade Database, string sql, params object[] parameters)
    {
      // if (sql.Contains("@")) return await ExecuteSqRawlWithParameters<T>(Database, sql, parameters);
      using (var _db = DbUtility.GetConnection(Database.GetDbConnection()) as IDbConnection)
      {
        _db.Open();

        var sqlParamateredString = SqlFormat(sql, parameters);

        return await _db.QueryAsync<T>(sqlParamateredString);

      }

    }
    /// <summary>
    /// use this when casting sql to models not in context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Database"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<T>> ExecuteSqlRawWithDapperAsync<T>(this DatabaseFacade Database, string sql, params Microsoft.Data.SqlClient.SqlParameter[] parameters)
    {
      using (var _db = DbUtility.GetConnection(Database.GetDbConnection()) as IDbConnection)
      {
        _db.Open();
        var args = new DynamicParameters(new { });
        foreach (var parameter in parameters)
        {
          if (parameter.SqlDbType == SqlDbType.Structured)
          {
            args.Add(parameter.ParameterName, ((DataTable)parameter.Value).AsTableValuedParameter(parameter.TypeName));
          }
          else
          {
            if (parameter.Value is System.DBNull)
              args.Add(parameter.ParameterName, null);
            else
              args.Add(parameter.ParameterName, parameter.Value);
          }
        }
        return await _db.QueryAsync<T>(sql, args);

      }

    }
    /// <summary>
    /// use this when casting sql to models not in context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Database"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<T>> ExecuteSqlRawWithDapperAsync<T>(this DatabaseFacade Database, string sql, params System.Data.SqlClient.SqlParameter[] parameters)
    {
      var convertedParams = new List<Microsoft.Data.SqlClient.SqlParameter>();
      foreach (var parameter in parameters)
        convertedParams.Add(new Microsoft.Data.SqlClient.SqlParameter(parameter.ParameterName, parameter.Value));

      return await ExecuteSqlRawWithDapperAsync<T>(Database, sql, convertedParams.ToArray());

    }
    /// <summary>
    /// use this when casting sql to models not in context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Database"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static IEnumerable<T> ExecuteSqlRawWithDapper<T>(this DatabaseFacade Database, string sql, params object[] parameters)
    {
      using (var _db = DbUtility.GetConnection(Database.GetDbConnection()) as IDbConnection)
      {
        _db.Open();
        var sqlParamateredString = SqlFormat(sql, parameters);

        return _db.Query<T>(sqlParamateredString);

      }

    }
    /// <summary>
    /// use this when casting sql to models not in context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Database"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static IEnumerable<T> ExecuteSqRawlWithDapper<T>(this DatabaseFacade Database, string sql, params Microsoft.Data.SqlClient.SqlParameter[] parameters)
    {
      using (var _db = DbUtility.GetConnection(Database.GetDbConnection()) as IDbConnection)
      {
        _db.Open();
        var args = new DynamicParameters(new { });
        foreach (var parameter in parameters)
        {
          if (parameter.SqlDbType == SqlDbType.Structured)
          {
            args.Add(parameter.ParameterName, ((DataTable)parameter.Value).AsTableValuedParameter(parameter.TypeName));
          }
          else
          {
            if (parameter.Value is System.DBNull)
              args.Add(parameter.ParameterName, null);
            else
              args.Add(parameter.ParameterName, parameter.Value);
          }
        }
        return _db.Query<T>(sql, args);
      }

    }
    /// <summary>
    /// use this when casting sql to models not in context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Database"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static IEnumerable<T> ExecuteSqRawlWithDapper<T>(this DatabaseFacade Database, string sql, params System.Data.SqlClient.SqlParameter[] parameters)
    {

      var convertedParams = new List<Microsoft.Data.SqlClient.SqlParameter>();
      foreach (var parameter in parameters)
        convertedParams.Add(new Microsoft.Data.SqlClient.SqlParameter(parameter.ParameterName, parameter.Value));

      return ExecuteSqRawlWithDapper<T>(Database, sql, convertedParams.ToArray());

    }

    /// <summary>
    /// use this when constructing dynamic expression
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static Expression<Func<T, object>> GetExpression<T>(this DbContext context, string propertyName)
    {
      var parameter = Expression.Parameter(typeof(T), "x");
      var property = Expression.Property(parameter, propertyName);
      var convert = Expression.Convert(property, typeof(object)); // Handle non-nullable types
      return Expression.Lambda<Func<T, object>>(convert, parameter);
    }

    /// <summary>
    /// Use this when executing sql commands with retry logic, handles timeouts and deadlocks and retries them
    /// </summary>
    /// <param name="databaseFacade"></param>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static async Task ExecuteSqlRawAsyncWithRetry(this DatabaseFacade databaseFacade, string sql, params object[] parameters)
    {
      const int maxRetries = 5; // Maximum number of retries
      int retries = 0; // Current retry count

      while (true)
      {
        try
        {
          await databaseFacade.ExecuteSqlRawAsync(sql, parameters);
          break; // If successful, exit the loop
        }
        catch (SqlException ex)
        {
          if ((ex.Number == -2 || ex.Number == 1205) && retries < maxRetries) // -2 is the error number for timeout, 1205 for deadlock
          {
            retries++;
            await Task.Delay(1000); // Wait for 1 second before retrying
          }
          else
          {
            throw; // Re-throw the exception if it's not a timeout or deadlock or if max retries have been reached
          }
        }
      }
    }

    public static DeferredQuery<T> ExecuteSqlPreparedFunction<T>(this DatabaseFacade Database, string sql, params object[] parameters)
    {
      var sqlParamateredString = SqlFormat(sql, parameters);
      return new DeferredQuery<T>(Database, sqlParamateredString);

    }

    public static DeferredQuery<T> ExecuteSqlPreparedFunction<T>(this DatabaseFacade Database, string sql, params Microsoft.Data.SqlClient.SqlParameter[] parameters)
    {

      var args = new DynamicParameters(new { });
      foreach (var parameter in parameters)
      {
        if (parameter.SqlDbType == SqlDbType.Structured)
        {
          args.Add(parameter.ParameterName, ((DataTable)parameter.Value).AsTableValuedParameter(parameter.TypeName));
        }
        else
        {
          if (parameter.Value is System.DBNull)
            args.Add(parameter.ParameterName, null);
          else
            args.Add(parameter.ParameterName, parameter.Value);
        }
      }
      return new DeferredQuery<T>(Database, sql, args);

    }

    private static SqlConnection GetConnection(DbConnection DbConnection)
    {
      return new SqlConnection(DbConnection.ConnectionString);
    }


  }
  public class DeferredQuery<T>
  {
    private string sql;
    private DynamicParameters? parameters;
    private DatabaseFacade database;

    public DeferredQuery(DatabaseFacade database, string sql, DynamicParameters parameters = null)
    {
      this.sql = sql;
      this.parameters = parameters;
      this.database = database;
    }

    public async Task<IEnumerable<T>?> ToListAsync()
    {
      using (var _db = DbUtility. GetConnection(database.GetDbConnection()) as IDbConnection)
      {
        try
        {
          _db.Open();
          return await _db.QueryAsync<T>(sql, parameters);
        }
        finally
        {
          _db.Close();
        }
      }
    }

    public async Task<T?> FirstOrDefaultAsync()
    {
      using (var _db = DbUtility. GetConnection(database.GetDbConnection()) as IDbConnection)
      {
        try
        {
          _db.Open();
          var result = await _db.QueryAsync<T>(sql, parameters);
          return result.FirstOrDefault();
        }
        finally
        {
          _db.Close();
        }
      }
    }
  }
}
