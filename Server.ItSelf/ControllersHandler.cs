using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server.ItSelf
{
    public class ControllersHandler : IHandler
    {
        private readonly Dictionary<string, Func<object>> routes = new Dictionary<string, Func<object>>();

        public ControllersHandler(Assembly assembly)
        {
            routes = assembly.GetTypes().Where(x => typeof(IController).IsAssignableFrom(x))
                .SelectMany(controller =>
                    controller.GetMethods()
                        .Select(method => new
                        {
                            Path = "/" + controller.Name + "/" + method.Name,
                            Activator = new Func<object>(() => method.Invoke(Activator.CreateInstance(controller), Array.Empty<object>()))
                        }))
                .ToDictionary(x => x.Path, x => x.Activator);
        }

        public void Handle(Request request, Stream networkStream)
        {
            if (routes.TryGetValue(request.Path, out var func))
            {
                HttpUtil.WriteStatus(System.Net.HttpStatusCode.OK, networkStream);
                WriteResponse(func(), networkStream);
            }
            else
            {
                HttpUtil.WriteStatus(System.Net.HttpStatusCode.NotFound, networkStream);
            }
        }

        public async Task HandleAsync(Request request, Stream networkStream)
        {
            if (routes.TryGetValue(request.Path, out var func))
            {
                HttpUtil.WriteStatus(System.Net.HttpStatusCode.OK, networkStream);
                await WriteResponseAsync(func(), networkStream);
            }
            else
            {
                await HttpUtil.WriteStatusAsync(System.Net.HttpStatusCode.NotFound, networkStream);
            }
        }

        private async Task WriteResponseAsync(object v, Stream stream)
        {
            if (v is string str)
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                writer.Write(str);
            }
            else if (v is byte[] bytes)
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            else if (v is Task task)
            {
                await task;
                var result = GetResult(task);
                if (result == null)
                    HttpUtil.WriteStatus(System.Net.HttpStatusCode.NoContent, stream);
                else
                    await WriteResponseAsync(result, stream);
            }
            else
            {
                await WriteResponseAsync(JsonConvert.SerializeObject(v), stream);
            }
        }

        private object GetResult(Task task)
        {
            if (!task.GetType().IsGenericType)
                return null;
            return task.GetType().GetProperty("Result").GetValue(task);
        }
        
        private void WriteResponse(object v, Stream stream)
        {
            if (v is string str)
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                writer.Write(str);
            }
            else if (v is byte[] bytes)
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            else
            {
                WriteResponse(JsonConvert.SerializeObject(v), stream);
            }
        }
    }
}
