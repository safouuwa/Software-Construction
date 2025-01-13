namespace Interfaces;
public interface ILoggableAction
{
    object GetDataBefore();
    object GetDataAfter();
}