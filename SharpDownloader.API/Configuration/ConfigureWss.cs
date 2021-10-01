using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SharpDownloader.API.MiddleWares;
using SharpDownloader.Downloader;
using SharpDownloader.Integration.Observer;
using SharpDownloader.Wss;

namespace SharpDownloader.API.Configuration
{
    public static class Configuration
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app,
                                                        PathString path,
                                                        ISocketHandler handler)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<WebSocketManagerMiddleware>(handler));
        }

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddSingleton<DownloadService>();
            services.AddHostedService<DownloadService>(p => p.GetRequiredService<DownloadService>());
            services.AddSingleton<ISocketManager, SocketManager>();
            services.AddTransient<ISocketHandler, SocketHandler>();
            // foreach(var type in Assembly.GetEntryAssembly().ExportedTypes)
            // {
            //     if(type.GetTypeInfo().BaseType == typeof(WsHandler))
            //     {
            //         services.AddSingleton(type);
            //     }
            // }

            return services;
        }
    }
    
}