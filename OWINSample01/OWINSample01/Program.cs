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

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use<MyMiddleware>();
            app.Use<MyOtherMiddleware>();

        }




        
    }

    public class MyMiddleware
    {
        private AppFunc _next;
        public MyMiddleware(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {

                // inbound
            IOwinContext context = new OwinContext(env);
            await context.Response.WriteAsync("<h1>Hello from My First Middleware</h1>");
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
