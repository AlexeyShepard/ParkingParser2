
namespace ParkingParser2.Core
{
    public enum RecordType
    {
        INFO = 0,
        WARNING,
        ERROR
    }

    /// <summary>
    /// Источник журналирования
    /// </summary>
    public enum Source
    {
        Parser = 0,
        UDP,
        System
    }
}
