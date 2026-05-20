using System_Resource_Monitor.Models;

namespace System_Resource_Monitor.Services;

public interface IProcessService
{
    IReadOnlyList<ProcessInfo> Sample();
}
