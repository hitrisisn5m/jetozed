﻿using FastTunnel.Core.Client;
using FastTunnel.Core.Extensions;
using FastTunnel.Core.Models;
using FastTunnel.Core.Sockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FastTunnel.Core.Handlers.Server
{
    public class SwapMessageHandler : IClientMessageHandler
    {
        public bool NeedRecive => false;

        ILogger _logger;

        public SwapMessageHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void HandlerMsg(FastTunnelServer server, Socket client, Message<JObject> msg)
        {
            var SwapMsg = msg.Content.ToObject<SwapMassage>();
            NewRequest request;

            _logger.LogDebug($"响应NewCustomer：{SwapMsg.msgId}");
            if (!string.IsNullOrEmpty(SwapMsg.msgId) && server.RequestTemp.TryGetValue(SwapMsg.msgId, out request))
            {
                server.RequestTemp.TryRemove(SwapMsg.msgId, out _);

                _logger.LogDebug($"SwapMassage：{SwapMsg.msgId}");

                new SocketSwap(request.CustomerClient, client, _logger, SwapMsg.msgId)
                   .BeforeSwap(() =>
                   {
                       if (request.Buffer != null) client.Send(request.Buffer);
                   })
                   .StartSwap();
            }
            else
            {
                // 未找到，关闭连接
                _logger.LogError($"未找到请求:{SwapMsg.msgId}");

                client.SendCmd(new Message<LogMassage> { MessageType = MessageType.Log, Content = new LogMassage(LogMsgType.Debug, $"未找到请求:{SwapMsg.msgId}") });

                try
                {
                    client.Shutdown(SocketShutdown.Both);
                }
                catch (Exception)
                {
                }

                client.Close();
            }
        }
    }
}
