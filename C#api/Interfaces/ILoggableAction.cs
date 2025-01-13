namespace Interfaces;
public interface ILoggableAction
{
    object _dataBefore { get; set; }
    object _dataAfter { get; set; }
}