using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MSDev.Tools.CommandRunner
{
    /// <summary>
    /// Abandoned Class
    /// </summary>
    public class CommandRunnerMiddleware
    {
        readonly RequestDelegate _next;

        readonly CommandRunnerOptions _options;
        public CommandRunnerMiddleware(RequestDelegate next,CommandRunnerOptions options)
        {
            _next = next;
            _options = options;
        }


        public async Task Invoke(HttpContext context)
        {
           //TODO: Use RouteHelper
            await _next(context);
        }

    }

    public static class CommandRunnerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCommandRunner(
            this IApplicationBuilder builder, CommandRunnerOptions options) => builder.UseMiddleware<CommandRunnerMiddleware>(options);
    }


    public class CommandRunnerOptions
    {
        public String RouteName{ get; set; }
    }
}
