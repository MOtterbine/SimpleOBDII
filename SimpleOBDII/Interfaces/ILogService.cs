using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace OS.OBDII.Interfaces;

public interface ILogService
{
    void AppendLog(LogLevel level, string logText);

}
