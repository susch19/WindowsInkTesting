using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DrawingTabletServer
{
    public class Program
    {
        public static DrawingManager DrawingManager;

        public static void Main(string[] args)
        {
            var existing = DrawingManager.ExistingDrawings();

            if (existing.Count > 0)
            {
                var res = ConsoleUi.CreateSelectionGridForArray("", existing.Concat(new string[] { "new" }), true);
                if (existing.Contains(res))
                    DrawingManager = new DrawingManager(res);
                else
                    DrawingManager = new DrawingManager();

            }

            CreateWebHostBuilder(args).UseUrls("http://[::1]:55332", "http://0.0.0.0:55332").Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
