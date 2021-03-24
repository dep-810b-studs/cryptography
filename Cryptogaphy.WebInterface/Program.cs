using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Cryptogaphy.WebInterface;

Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
    .Build()
    .Run();