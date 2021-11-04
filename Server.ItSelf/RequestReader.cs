using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Server.ItSelf
{
    public class ServerException : Exception
    {
        public ServerException(string message) : base (message)
        {
        }
    }

    internal class RequestReader
    {
        internal static Request Read(StreamReader reader)
        {
            if (reader.EndOfStream)
                throw new ServerException("No headers");
            var firstLine = reader.ReadLine();
            var l1 = firstLine.Split(" ");
            if (l1.Length != 3)
                throw new ServerException("Incorrect header");
            var method = GetMethod(l1[0]);

            for (string line = null; line != string.Empty; line = reader.ReadLine())
                ;
            return new Request(l1[1], method);
        }

        internal static async Task<Request> ReadAsync(StreamReader reader)
        {
            if (reader.EndOfStream)
                throw new ServerException("No headers");
            var firstLine = await reader.ReadLineAsync();
            var l1 = firstLine.Split(" ");
            if (l1.Length != 3)
                throw new ServerException("Incorrect header");
            var method = GetMethod(l1[0]);

            for (string line = null; line != string.Empty; line = await reader.ReadLineAsync())
                ;
            return new Request(l1[1], method);
        }

        private static HttpMethod GetMethod(string method)
        {
            if (method == "GET")
                return HttpMethod.Get;
            return HttpMethod.Post;
        }
    }
}