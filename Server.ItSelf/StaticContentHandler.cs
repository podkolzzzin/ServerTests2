using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Server.ItSelf
{
    public sealed class StaticContentHandler : IHandler
    {
        private readonly string path;

        public StaticContentHandler(string path)
        {
            this.path = Path.Combine(Environment.CurrentDirectory, path);
        }

        public void Handle(Request request, Stream networkStream)
        {
            try
            {
                HandleInternal(request, networkStream);
            }
            catch (IOException)
            {
                HttpUtil.WriteStatus(HttpStatusCode.InternalServerError, networkStream);
            }
        }

        public async Task HandleAsync(Request request, Stream networkStream)
        {
            if (request.Method == System.Net.Http.HttpMethod.Get)
            {
                var filePath = Path.Combine(path, request.Path.Substring(1));
                if (!File.Exists(filePath))
                {
                    await HttpUtil.WriteStatusAsync(HttpStatusCode.NotFound, networkStream);
                }
                else
                {
                    using var fileStream = File.OpenRead(filePath);
                    HttpUtil.WriteStatus(HttpStatusCode.OK, networkStream);
                    await fileStream.CopyToAsync(networkStream);
                }
            }
            else
                await HttpUtil.WriteStatusAsync(HttpStatusCode.MethodNotAllowed, networkStream);
        }

        private void HandleInternal(Request request, Stream networkStream)
        {
            if (request.Method == System.Net.Http.HttpMethod.Get)
            {
                var filePath = Path.Combine(path, request.Path.Substring(1));
                if (!File.Exists(filePath))
                {
                    HttpUtil.WriteStatus(HttpStatusCode.NotFound, networkStream);
                }
                else
                {
                    using var fileStream = File.OpenRead(filePath);
                    HttpUtil.WriteStatus(HttpStatusCode.OK, networkStream);
                    fileStream.CopyTo(networkStream);
                }
            }
            else
                HttpUtil.WriteStatus(HttpStatusCode.MethodNotAllowed, networkStream);
        }
    }
}
