﻿using FastTunnel.Core.Client;
using FastTunnel.Core.Forwarder;
using FastTunnel.Core.Forwarder.MiddleWare;
using FastTunnel.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Configuration;

namespace FastTunnel.Core.MiddleWares
{
    public class FastTunnelClientHandler
    {
        ILogger<FastTunnelClientHandler> logger;
        FastTunnelServer fastTunnelServer;

        public FastTunnelClientHandler(ILogger<FastTunnelClientHandler> logger, FastTunnelServer fastTunnelServer)
        {
            this.logger = logger;
            this.fastTunnelServer = fastTunnelServer;
        }

        public async Task Handle(HttpContext context, Func<Task> next)
        {
            if (!context.WebSockets.IsWebSocketRequest
                || !context.Request.Headers.TryGetValue(FastTunnelConst.FASTTUNNEL_FLAG, out var version)
                || !context.Request.Headers.TryGetValue(FastTunnelConst.FASTTUNNEL_TYPE, out var type))
            {
                await next();
                return;
            };

            await handleClient(context, next);
        }

        private async Task handleClient(HttpContext context, Func<Task> next)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            var tunnelClient = new TunnelClient(logger, fastTunnelServer).SetSocket(webSocket);

            try
            {
                logger.LogInformation($"客户端连接 {context.TraceIdentifier}:{context.Connection.RemoteIpAddress}");
                await tunnelClient.ReviceAsync(CancellationToken.None);
            }
            catch (Exception)
            {
                logger.LogInformation($"客户端关闭 {context.TraceIdentifier}:{context.Connection.RemoteIpAddress}");
            }
        }
    }
}
