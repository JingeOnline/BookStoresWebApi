using BookStoresWebApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BookStoresWebApi.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly BookStoresDBContext _context;

        //构造函数获取必要的信息，并传递给基类的构造函数。
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            BookStoresDBContext context) : base(options, logger, encoder, clock)
        {
            _context = context;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //如果请求头中不存在Authorization字段，那么直接返回验证失败。
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Authorization header was not found.");
            }
            else
            {
                try
                {
                    //取出请求头中的Authorization字段内容，并根据字符串创建一个AuthenticationHeaderValue对象
                    var authenticationHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                    //把Base64编码的内容解析出来
                    var bytes = Convert.FromBase64String(authenticationHeader.Parameter);
                    string authenticationString = Encoding.UTF8.GetString(bytes);
                    //邮箱和密码之间使用了冒号作为分隔符，把二者分开
                    string[] credentials = authenticationString.Split(":");
                    string email = credentials[0];
                    string password = credentials[1];
                    //去数据库中查询该邮箱和密码是否存在且正确
                    User user = _context.Users.Where(user =>
                          user.EmailAddress == email && user.Password == password)
                          .FirstOrDefault();
                    if (user == null)
                    {
                        return AuthenticateResult.Fail("Invalid username or password");
                    }
                    else
                    {
                        //创建Identity
                        Claim[] claims = new[]
                        {
                            new Claim( ClaimTypes.Name, user.EmailAddress),
                        };
                        ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                        AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);

                        return AuthenticateResult.Success(ticket);
                    }
                }
                catch (Exception)
                {

                    //failureMessage并不会显示，只会返回401 Unauthorized
                    return AuthenticateResult.Fail("Error occured, authenticate fail.");
                }
                
            }

            
        }
    }
}