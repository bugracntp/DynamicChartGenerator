using GenericCharts.Shared.ComplexTypes;
using GenericCharts.Shared.Concrete;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Npgsql;

namespace GenericCharts.DataAccess;

public interface IChartDataAccess
{
    public Response<List<string>>  GetPGAdminTableNames(string connectionString);
    public Response<List<string>> GetMSSqlTableNames(string connectionString);
    public Response<List<string>> GetMySqlTableNames(string connectionString);
    
    public Response<List<object>> GetVariableFromPGAdmin(string connectionString, string objectName);
    public Response<List<object>> GetVariableFromSqlServer(string connectionString, string objectName);
    public Response<List<object>> GetVariableFromMySql(string connectionString, string objectName);
}

internal class ChartDataAccess: IChartDataAccess
{
    private IChartDataAccess _chartDataAccessImplementation;

    public Response<List<string>> GetPGAdminTableNames(string connectionString)
    {
        var tableNames = new List<string>();
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                const string query = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableNames.Add(reader.GetString(0));
                        }
                    }
                }
            }
        }
        catch (NpgsqlException npgsqlEx) {
            return new Response<List<string>>(ResponseCode.BadRequest, $"PostgreSQL işlemi sırasında bir hata oluştu: {npgsqlEx.Message}");
        }
        catch (InvalidOperationException invalidEx) {
            return new Response<List<string>>(ResponseCode.BadRequest,  $"PostgreSQL bağlantısı sırasında bir hata oluştu: {invalidEx.Message}");
        }
        catch (Exception ex) {
            return new Response<List<string>>(ResponseCode.InternalServerError, $"Beklenmeyen bir hata oluştu: {ex.Message}");
        }
        
        return new Response<List<string>>(ResponseCode.Success, tableNames);
    }

    public Response<List<string>> GetMSSqlTableNames(string connectionString)
    {
        var tableNames = new List<string>();
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                const string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableNames.Add(reader.GetString(0));
                        }
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            return new Response<List<string>>(ResponseCode.BadRequest, $"Veritabanı işlemi sırasında bir hata oluştu: {sqlEx.Message}");
        }
        catch (InvalidOperationException invalidEx)
        {
            return new Response<List<string>>(ResponseCode.BadRequest, $"Veritabanı bağlantısı sırasında bir hata oluştu: {invalidEx.Message}");
        }
        catch (Exception ex)
        {
            return new Response<List<string>>(ResponseCode.InternalServerError, $"Beklenmeyen bir hata oluştu: {ex.Message}");
        }

        return new Response<List<string>>(ResponseCode.Success, tableNames);
    }

    public Response<List<string>> GetMySqlTableNames(string connectionString)
    {
        var tableNames = new List<string>();
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                const string query = "SHOW TABLES";

                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableNames.Add(reader.GetString(0));
                        }
                    }
                }
            }
        }
        catch (MySqlException mySqlEx) {
            return new Response<List<string>>(ResponseCode.InternalServerError, $"MySQL işlemi sırasında bir hata oluştu: {mySqlEx.Message}");
        }
        catch (InvalidOperationException invalidEx)
        {
            return new Response<List<string>>(ResponseCode.InternalServerError, $"MySQL bağlantısı sırasında bir hata oluştu: {invalidEx.Message}");
        }
        catch (Exception ex)
        {
            return new Response<List<string>>(ResponseCode.BadRequest, $"Beklenmeyen bir hata oluştu: {ex.Message}");
        }

        return new Response<List<string>>(ResponseCode.Success, tableNames);
    }
public Response<List<object>> GetVariableFromPGAdmin(string connectionString, string tableName)
{
    var result = new List<object>();
    try
    {
        using (var cnn = new NpgsqlConnection(connectionString))
        {
            cnn.Open();
            using (var cmd = new NpgsqlCommand($"SELECT * FROM public.\"{tableName}\"", cnn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetName(i), reader.GetValue(i));
                        }
                        result.Add(row);
                    }
                }
            }
        }
    }
    catch (NpgsqlException npgsqlEx)
    {
        return new Response<List<object>>(ResponseCode.BadRequest, $"PostgreSQL hatası: {npgsqlEx.Message}");
    }
    catch (Exception ex)
    {
        return new Response<List<object>>(ResponseCode.InternalServerError, $"Beklenmeyen bir hata oluştu: {ex.Message}");
    }

    return new Response<List<object>>(ResponseCode.Success, result);
}

public Response<List<object>> GetVariableFromSqlServer(string connectionString, string tableName)
{
    var result = new List<object>();
    try
    {
        using (var cnn = new SqlConnection(connectionString))
        {
            cnn.Open();
            using (var command = cnn.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {tableName}";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetName(i), reader.GetValue(i));
                        }
                        result.Add(row);
                    }
                }
            }
        }
    }
    catch (SqlException sqlEx)
    {
        return new Response<List<object>>(ResponseCode.BadRequest, $"SQL Server hatası: {sqlEx.Message}");
    }
    catch (Exception ex)
    {
        return new Response<List<object>>(ResponseCode.InternalServerError, $"Beklenmeyen bir hata oluştu: {ex.Message}");
    }

    return new Response<List<object>>(ResponseCode.Success, result);
}

public Response<List<object>> GetVariableFromMySql(string connectionString, string tableName)
{
    var result = new List<object>();
    try
    {
        using (var cnn = new MySqlConnection(connectionString))
        {
            cnn.Open();
            using (var command = cnn.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM `{tableName}`"; 
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetName(i), reader.GetValue(i));
                        }
                        result.Add(row);
                    }
                }
            }
        }
    }
    catch (MySqlException mySqlEx)
    {
        return new Response<List<object>>(ResponseCode.BadRequest, $"MySQL hatası: {mySqlEx.Message}");
    }
    catch (Exception ex)
    {
        return new Response<List<object>>(ResponseCode.InternalServerError, $"Beklenmeyen bir hata oluştu: {ex.Message}");
    }

    return new Response<List<object>>(ResponseCode.Success, result);
}
}