﻿using FastTunnel.Core.Config;
using FastTunnel.Core.Client;
using FastTunnel.Core.Filters;
using FastTunnel.Core.Global;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using FastTunnel.Core.Models;

namespace FastTunnel.Core.Services
{
    public class ServiceFastTunnelServer : IHostedService
    {
        readonly ILogger<ServiceFastTunnelServer> _logger;
        readonly IAuthenticationFilter _authenticationFilter;
        FastTunnelServer _server;

        public ServiceFastTunnelServer(
            ILogger<ServiceFastTunnelServer> logger,
            FastTunnelServer fastTunnelServer,
            IAuthenticationFilter authenticationFilter)
        {
            _logger = logger;
            _authenticationFilter = authenticationFilter;
            _server = fastTunnelServer;
            //AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                _logger.LogError("【UnhandledException】" + e.ExceptionObject);
                _logger.LogError("【UnhandledException】" + JsonConvert.SerializeObject(e.ExceptionObject));
                var type = e.ExceptionObject.GetType();
                _logger.LogError("ExceptionObject GetType " + type);
            }
            catch
            {
            }
        }

        private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            if (e.Exception is DirectoryNotFoundException)
            {
                // nlog第一次找不到文件的错误，跳过
            }
            else if (e.Exception is PlatformNotSupportedException)
            {
                // log4net
            }
            else if (e.Exception is IOException && e.Exception.Source == "System.Net.Security")
            {
            }
            else
            {
                _logger.LogError(e.Exception, "【FirstChanceException】");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                FastTunnelGlobal.AddFilter(_authenticationFilter);

                try
                {
                    _server.Run();
                }
                catch (Exception ex)
                {
                    // NLog: catch any exception and log it.
                    _logger.LogError(ex, "Server Error");
                    Console.WriteLine(ex);
                }

            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                _server.Stop();
            }, cancellationToken);
        }
    }
}
