﻿using FastTunnel.Core.Config;
using FastTunnel.Core.Client;
using FastTunnel.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastTunnel.Core.Sockets;

namespace FastTunnel.Core.Handlers.Client
{
    public class HttpRequestHandler : IClientHandler
    {
        public void HandlerMsg(FastTunnelClient cleint, Message<JObject> Msg)
        {
            var request = Msg.Content.ToObject<NewCustomerMassage>();
            var connecter = new DnsSocket(cleint.Server.ServerAddr, cleint.Server.ServerPort);

            connecter.Connect();
            connecter.Send(new Message<SwapMassage> { MessageType = MessageType.C_SwapMsg, Content = new SwapMassage(request.MsgId) });

            var localConnecter = new DnsSocket(request.WebConfig.LocalIp, request.WebConfig.LocalPort);

            try
            {
                localConnecter.Connect();
            }
            catch (SocketException sex)
            {
                localConnecter.Close();
                if (sex.ErrorCode == 10061)
                {
                    // 内网的站点不存在或无法访问
                    string statusLine = "HTTP/1.1 200 OK\r\n";
                    string responseHeader = "Content-Type: text/html\r\n";
                    byte[] responseBody;
                    responseBody = Encoding.UTF8.GetBytes(TunnelResource.Page_NoSite);

                    connecter.Send(Encoding.UTF8.GetBytes(statusLine));
                    connecter.Send(Encoding.UTF8.GetBytes(responseHeader));
                    connecter.Send(Encoding.UTF8.GetBytes("\r\n"));
                    connecter.Send(responseBody);

                    connecter.Socket.Disconnect(false);
                    connecter.Socket.Close();
                    return;
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                localConnecter.Close();
                throw;
            }

            new SocketSwap(connecter.Socket, localConnecter.Socket).StartSwap();
        }
    }
}
