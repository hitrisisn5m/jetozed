﻿using FastTunnel.Core.Core;
using FastTunnel.Core.Extensions;
using FastTunnel.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FastTunnel.Core.Handlers.Server
{
    public class SwapMsgHandler : IServerHandler
    {
        public bool NeedRecive => false;

        ILogger _logger;

        public SwapMsgHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void HandlerMsg(FastTunnelServer server, Socket client, Message<JObject> msg)
        {
            var SwapMsg = msg.Content.ToObject<SwapMassage>();
            NewRequest request;

            if (!string.IsNullOrEmpty(SwapMsg.msgId) && server.newRequest.TryGetValue(SwapMsg.msgId, out request))
            {
                // Join
                Task.Run(() =>
                {
                    (new SocketSwap(request.CustomerClient, client))
                        .BeforeSwap(() => { if (request.Buffer != null) client.Send(request.Buffer); })
                        .StartSwap();
                });
            }
            else
            {
                // 未找到，关闭连接
                _logger.LogError($"未找到请求:{SwapMsg.msgId}");
                client.Send(new Message<LogMassage> { MessageType = MessageType.Log, Content = new LogMassage(LogMsgType.Debug, $"未找到请求:{SwapMsg.msgId}") });
            }
        }
    }
}
