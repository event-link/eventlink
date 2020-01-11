using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace EventLink.Auth
{

    public class AuthException : Exception
    {
        public AuthException() { }
        public AuthException(string message) : base(message) { }
        public AuthException(string message, Exception inner) : base(message, inner) { }
    }

    public class AuthBadCredentialsException : AuthException
    {
        public AuthBadCredentialsException() { }
        public AuthBadCredentialsException(string message) : base(message) { }
        public AuthBadCredentialsException(string message, Exception inner) : base(message, inner) { }
    }

    public class AuthUserNotFoundException : AuthException
    {
        public AuthUserNotFoundException() { }
        public AuthUserNotFoundException(string message) : base(message) { }
        public AuthUserNotFoundException(string message, Exception inner) : base(message, inner) { }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
