# J_EFWrapper
EF Core Wrappers and Helpers


See example for code examples

Simplies ef core to be able to use the database to execute queries and cast to models not contained in context, like EF framework could before.

Solves this by using Dapper and extension methods on Database Facade

FirstOrDefaultAsync
```csharp
      string sql = "SELECT top 1 * FROM Employees WHERE Id = @Id";
      var parameters = new { Id = 1};
      var employee = await _dbContext.Database.ExecuteSqlPreparedFunction<Employee>(sql, parameters).FirstOrDefaultAsync();

```


ToListAsync
```csharp
      string sql = "SELECT top 20 * FROM Employees";
      var employees = await _dbContext.Database.ExecuteSqlPreparedFunction<Employee>(sql, new object[] { }).ToListAsync();
```
