using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace CommandRunner
{
    public class Program
    {
        public static void Main(String[] args)
        {
            IWebHost host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:10916")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
