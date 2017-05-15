using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace CommandRunner.Helpers
{
    public static class SessionHelper
    {

        public static async Task ValidateAsync(CookieValidatePrincipalContext context)
        {

            ISession session = context.HttpContext.Session;
            String cookieUserName = context.Principal.FindFirst(p => p.ValueType == ClaimTypes.Name)?.Value;

            //TODO: 根据cookie同步session，缺少session的判断....
            if (!String.IsNullOrEmpty(cookieUserName))
            {
                session.SetString("username", cookieUserName);
            }


        }
    }
}
