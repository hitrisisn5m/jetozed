﻿using FastTunnel.Core.Client;
using FastTunnel.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastTunnel.Core.Config
{
    public class DefaultClientConfig : IClientConfig
    {
        public SuiDaoServer Server { get; set; }

        public IEnumerable<WebConfig> Webs { get; set; }

        public IEnumerable<SSHConfig> SSH { get; set; }
    }
}