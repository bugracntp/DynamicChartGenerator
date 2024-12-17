public class ChartRequest
{
    public int? DatabaseType { get; set; }
    public string? ServerName { get; set; }
    public string? DatabaseName { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public int? Port { get; set; }
    public string? TableName { get; set; }
    public int? ChartType { get; set; } 
}
public enum DatabaseType
{
    SqlServer,
    MySql,
    PostgreSql
}