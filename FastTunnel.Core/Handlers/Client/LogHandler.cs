﻿using FastTunnel.Core.Config;
using FastTunnel.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using FastTunnel.Core.Extensions;
using FastTunnel.Core.Client;
using System.Threading.Tasks;

namespace FastTunnel.Core.Handlers.Client
{
    public class LogHandler : IClientHandler
    {
        ILogger<LogHandler> _logger;

        public LogHandler(ILogger<LogHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandlerMsgAsync<T>(FastTunnelClient cleint, T Msg)
            where T : TunnelMassage
        {
            try
            {
                await Task.Yield();
                var msg = Msg as LogMassage;

                switch (msg.MsgType)
                {
                    case LogMsgType.Info:
                        _logger.LogInformation($"[Server Info]:{msg.Msg}");
                        break;
                    case LogMsgType.Error:
                        _logger.LogError($"[Server Error]:{msg.Msg}");
                        break;
                    case LogMsgType.Debug:
                        _logger.LogDebug($"[Server Debug]:{msg.Msg}");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }
    }
}
