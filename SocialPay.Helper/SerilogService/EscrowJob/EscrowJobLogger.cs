﻿
using Serilog;
using SocialPay.Helper.Configurations;
using System;
using Microsoft.Extensions.Configuration;

namespace SocialPay.Helper.SerilogService.Escrow
{
    
    public class EscrowJobLogger
    {
        public IConfiguration Configuration { get; }
        public EscrowJobLogger(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void LogRequest(string message, bool isError)
        {
            var options = Configuration.GetSection(nameof(SerilogConfiguration)).Get<SerilogConfiguration>();
            Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .Enrich.FromLogContext()
           // Add this line:
           .WriteTo.File(
              options.escrowjoblogger,
               outputTemplate: "{Timestamp:o} [{Level:u3}] ({SourceContext}) {Message}{NewLine}{Exception}",
               fileSizeLimitBytes: 1_000_000,
               rollingInterval: RollingInterval.Day,
               rollOnFileSizeLimit: true,
               shared: true,
               flushToDiskInterval: TimeSpan.FromSeconds(1))
           .CreateLogger();

            if (isError)
            {
                Log.Logger.Error(message);
            }
            else
            {
                Log.Logger.Information(message);
            }
        }

    }


}