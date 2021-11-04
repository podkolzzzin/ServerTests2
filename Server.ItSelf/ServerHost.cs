using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server.ItSelf
{
    public class ServerHost
    {
        private readonly IHandler handler;

        public ServerHost(IHandler handler)
        {
            this.handler = handler;
        }

        public void StartV2()
        {
            Console.WriteLine("Server started");
            var listener = new TcpListener(System.Net.IPAddress.Any, 80);
            listener.Start();
            while (true)
            {
                ProcessClient(listener.AcceptTcpClient());
            }
        }

        public void StartV1()
        {
            Console.WriteLine("Server started");
            var listener = new TcpListener(System.Net.IPAddress.Any, 80);
            listener.Start();
            while (true)
            {
                using(var client = listener.AcceptTcpClient())
                {
                    try
                    {
                        HandleClient(client);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unable to handle client: {ex}");
                    }
                }
            }
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Server started");
            var listener = new TcpListener(System.Net.IPAddress.Any, 80);
            listener.Start();
            while (true)
            {
                await ProcessClientAsync(listener.AcceptTcpClient());
            }
        }

        private async Task ProcessClientAsync(TcpClient tcpClient)
        {
            using (tcpClient)
                await HandleClientAsync(tcpClient);
        }

        private void ProcessClient(TcpClient tcpClient)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                using (tcpClient)
                {
                    try
                    {
                        HandleClient(tcpClient);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unable to handle client: {ex}");
                    }
                }
            });
        }

        private void HandleClient(TcpClient client)
        {
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            {
                var request = RequestReader.Read(reader);
                handler.Handle(request, stream);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            {
                var request = await RequestReader.ReadAsync(reader);
                await handler.HandleAsync(request, stream);
            }
        }
    }

    public interface IHandler
    {
        void Handle(Request request, Stream networkStream);

        Task HandleAsync(Request request, Stream networkStream);
    }
}
