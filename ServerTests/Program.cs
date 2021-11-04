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
            if (args.Length == 1)
            {
                if (args[0] == "1")
                    server.StartV1();
                else if (args[0] == "2")
                    server.StartV2();
            }
            await server.StartAsync();
        }
    }
}
