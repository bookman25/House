using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace HouseService.Services
{
    public static class LogHelper
    {
        public static ILogger<AutomationEngine> DefaultLogger { get; set; }

        private static int RequestId;

        public static IDisposable PushRequestId(string prefix)
        {
            Interlocked.Increment(ref RequestId);
            return LogContext.PushProperty("RequestId", $"[{prefix}-{RequestId}]");
        }
    }
}
