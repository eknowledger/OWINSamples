using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;

namespace OWINSample01
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    class Program
    {
        static void Main(string[] args)
        {
            WebApp.Start<Startup>("http://localhost:8080");
            Console.WriteLine("Server Started; Press enter to Quit");
            Console.ReadLine();
        }
    }
    public static class AppBuilderExtensions
    {
        public static void UseMyMiddleware(this IAppBuilder app, string greetings)
        {
            app.Use<MyMiddleware>(greetings);
        }

        public static void UseMyOtherMiddleware(this IAppBuilder app)
        {
            app.Use<MyOtherMiddleware>();
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseMyMiddleware("Hawdy from 1st middleware");
            app.UseMyOtherMiddleware();

        }

    }

    public class MyMiddleware
    {
        private AppFunc _next;
        private readonly string _greetings;

        public MyMiddleware(AppFunc next, string greetings)
        {
            _next = next;
            _greetings = greetings;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {

                // inbound
            IOwinContext context = new OwinContext(env);
            await context.Response.WriteAsync(string.Format("<h1>{0}</h1>", _greetings));
                // call next middleware func
            await _next.Invoke(env);

        }
    }
    public class MyOtherMiddleware
    {
        private AppFunc _next;
        public MyOtherMiddleware(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {

                // inbound
            IOwinContext context = new OwinContext(env);
            await context.Response.WriteAsync("<h1>Hello from My 2nd Middleware</h1>");
                // call next middleware func
            await _next.Invoke(env);

        }
    }
}
