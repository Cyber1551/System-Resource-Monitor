using System_Resource_Monitor.Models;

namespace System_Resource_Monitor.Services;

public interface ICpuService : IDisposable
{
    CpuSample Sample();
}
