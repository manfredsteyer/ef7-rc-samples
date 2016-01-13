using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFSample
{
    public static class DbContextExtensions
    {
        public static void LogToConsole(this DbContext context)
        {
            var contextServices = ((IInfrastructure<IServiceProvider>)context).Instance;
            var loggerFactory = contextServices.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddConsole(LogLevel.Verbose);
        }
    }
}
