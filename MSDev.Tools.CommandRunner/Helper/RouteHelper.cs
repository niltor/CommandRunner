using System;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace MSDev.Tools.CommandRunner.Helper
{
    public class RouteHelper
    {

        public RouteHelper()
        {

        }


        public static String Router(HttpContext context, String RouteName)
        {
            PathString path = context.Request.Path;
            String[] pathArray = path.Value.ToLower().Split('/');
            String routeName = pathArray?[1];
            String methodName = pathArray?[2];

            if (routeName != RouteName)
            {
                return null;
            }

            //TODO: run method via reflection ,see webapi project
            //TODO: get or post 
            
            //QueryString parameter = context.Request.QueryString;

            if (context.Request.Method.ToLower() == "post")
            {
                IFormCollection form = context.Request.Form;

            }

            if (path.Value.Trim('/').ToLower() == "test")
            {
                context.Response.ContentType = "text/html";

                String html = "<h1>hello</h1>";
                context.Response.WriteAsync(html);
            }
            return "";
        }

        public void GetClass(String className)
        {
            
        }

        public void ParseQueryString(String queryString)
        {

        }

        public void ParseFormData(IFormCollection form)
        {

        }

    }
}
