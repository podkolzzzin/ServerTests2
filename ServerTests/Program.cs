using Server.ItSelf;
using System;
using System.Threading.Tasks;

namespace ServerTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new ServerHost(new ControllersHandler(typeof(Program).Assembly));
            //server.StartV1();
            //server.StartV2();
            await server.StartAsync();
        }
    }
}
