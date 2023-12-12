using J_EfWrapper.Code;
using J_EfWrapper.Examples.Models;
using Microsoft.EntityFrameworkCore;

namespace J_EfWrapper.Examples
{
  public class Example
  {
    DbContext _dbContext;
    public Example(DbContext context)
    {
      _dbContext = context;
    }

    public async Task FirstOrDefaultAsync()
    {
      string sql = "SELECT top 1 * FROM Employees WHERE Id = @Id";
      var parameters = new { Id = 1};
      var employee = await _dbContext.Database.ExecuteSqlPreparedFunction<Employee>(sql, parameters).FirstOrDefaultAsync();
    }


    public async Task ToListAsync()
    {
      string sql = "SELECT top 20 * FROM Employees";
      var employees = await _dbContext.Database.ExecuteSqlPreparedFunction<Employee>(sql, new object[] { }).ToListAsync();
    }

    public async Task ExecuteSqlRawWithDapperAsync()
    {
      string sql = "SELECT * FROM Employees";
      var employees = await _dbContext.Database.ExecuteSqlRawWithDapperAsync<Employee>(sql);
    }

    public async Task ExecuteSqlRawWithDapperAsyncWithStringFormat()
    {
      string sql = "SELECT Id, Name, Price, Category FROM Products WHERE Category = {0}";
      var parameters = new { Category = "Electronics" };
      var products = await _dbContext.Database.ExecuteSqlRawWithDapperAsync<Product>(sql, new object[] { "Electronics" });
    }

    public async Task ExecuteSqlRawWithDapperAsyncWithParameters()
    {
      string sql = "SELECT Id, Name, Price, Category FROM Products WHERE Category = @Category";
      var parameters = new { Category = "Electronics" };
      var products = await _dbContext.Database.ExecuteSqlRawWithDapperAsync<Product>(sql, parameters);
    }

    public async Task ExecuteSqlRawWithDapperAsyncWithSqlParameters()
    {
      string sql = "SELECT Id, OrderDate, CustomerName, TotalAmount FROM Orders WHERE CustomerID = @CustomerID";
      var parameters = new[]
          {
             new System.Data.SqlClient.SqlParameter("@CustomerID", 12345)
          };
      var orders = await _dbContext.Database.ExecuteSqlRawWithDapperAsync<Order>(sql, parameters);

    }

  }
}
