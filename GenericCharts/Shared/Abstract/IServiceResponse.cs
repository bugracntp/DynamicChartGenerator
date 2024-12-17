using GenericCharts.Shared.ComplexTypes;

namespace GenericCharts.Shared.Abstract
{
    public interface IServiceResponse
    {
        ResponseCode ResponseCode { get; }
        string Message { get; }
    }
}
