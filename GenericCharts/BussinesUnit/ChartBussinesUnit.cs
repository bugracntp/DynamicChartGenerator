using GenericCharts.DataAccess;
using GenericCharts.Models;
using GenericCharts.Shared.ComplexTypes;
using GenericCharts.Shared.Concrete;
using Microsoft.Data.SqlClient;
using Npgsql;

public interface IChartBusinessUnit
{
    public Response<List<object>>GetChartData(ChartRequest request);
    public  Response<List<string>> GetTableNames(GetTablesDto request);
}
public class ChartBusinessUnit : IChartBusinessUnit
{
    private readonly IChartDataAccess _chartDataAccess;
    public ChartBusinessUnit(IChartDataAccess chartDataAccess)
    {
        _chartDataAccess = chartDataAccess;
    }
    
    public Response<List<object>> GetChartData(ChartRequest request)
    {
        switch (request.DatabaseType)
        {
            case (int)DatabaseType.SqlServer:
                return _chartDataAccess.GetVariableFromSqlServer("Server=" + request.ServerName + ";Database=" + request.DatabaseName + ";User Id=" + request.UserName + ";Password=" + request.Password + ";", request.TableName);
            case (int)DatabaseType.MySql:
                return _chartDataAccess.GetVariableFromMySql("Server=" + request.ServerName + ";Database=" + request.DatabaseName + ";Uid=" + request.UserName + ";Pwd=" + request.Password + ";", request.TableName);
            case (int)DatabaseType.PostgreSql:
                return _chartDataAccess.GetVariableFromPGAdmin("Host=" + request.ServerName + ";Port=" + request.Port + ";Database=" + request.DatabaseName + ";Username=" + request.UserName + ";Password=" + request.Password + ";", request.TableName);
        }

        return new Response<List<object>>(ResponseCode.InternalServerError);
    }
    
    public Response<List<string>> GetTableNames(GetTablesDto request)
    {
        switch (request.DatabaseType)
        {
            case (int)DatabaseType.SqlServer:
                return _chartDataAccess.GetMSSqlTableNames("Server=" + request.ServerName + ";Database=" + request.DatabaseName + ";User Id=" + request.UserName + ";Password=" + request.Password + ";");
            case (int)DatabaseType.MySql:
                return _chartDataAccess.GetMySqlTableNames("Server=" + request.ServerName + ";Database=" + request.DatabaseName + ";Uid=" + request.UserName + ";Pwd=" + request.Password + ";");
            case (int)DatabaseType.PostgreSql:
                return _chartDataAccess.GetPGAdminTableNames("Host=" + request.ServerName + ";Port=" + request.Port + ";Database=" + request.DatabaseName + ";Username=" + request.UserName + ";Password=" + request.Password + ";");
        }
        return new Response<List<string>>(ResponseCode.InternalServerError);
    }
    

}