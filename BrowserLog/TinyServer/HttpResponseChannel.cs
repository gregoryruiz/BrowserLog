﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BrowserLog.TinyServer
{
    public class HttpResponseChannel
    {
        private readonly TcpClient _tcpClient;

        public HttpResponseChannel(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public void Send(object obj)
        {
            var content = Encoding.UTF8.GetBytes(obj.ToString());
            var stream = _tcpClient.GetStream();
            var writeTask = stream.WriteAsync(content, 0, content.Length);
            writeTask.ContinueWith(t => stream.Flush());
        }

        public void Close()
        {
            _tcpClient.Close();
        }
    }
}
