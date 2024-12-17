using GenericCharts.Models;
using GenericCharts.Shared.ComplexTypes;
using GenericCharts.Shared.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace GenericCharts.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChartController : ControllerBase
{
    private readonly IChartBusinessUnit _chartBusinessUnit;

    public ChartController(IChartBusinessUnit chartBusinessUnit)
    {
        _chartBusinessUnit = chartBusinessUnit;
    }
    
    [HttpPost]
    [Route("GetChartData")]
    public Response<List<object>> GetChartData([FromBody] ChartRequest request)
    {
        var data = _chartBusinessUnit.GetChartData(request);
        return data;
    }
    
    [HttpPost]
    [Route("GetTablesFromDatabase")]
    public Response<List<string>> GetTablesFromDatabase([FromBody] GetTablesDto request)
    {
        var data = _chartBusinessUnit.GetTableNames(request);
        return data;
    }
}