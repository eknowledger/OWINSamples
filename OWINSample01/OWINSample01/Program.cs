using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var middleware = new Func<AppFunc, AppFunc>(MyMiddleware);
            var otherMiddleware = new Func<AppFunc, AppFunc>(MyOtherMiddleware);
            app.Use(middleware);
            app.Use(otherMiddleware);
  
        }

        public AppFunc MyMiddleware(AppFunc next)
        {
            AppFunc func = async (IDictionary<string, object> env) =>
            {
                // inbound
                var response = env["owin.ResponseBody"] as Stream;
                using (var writer = new StreamWriter(response))
                {
                    await writer.WriteAsync("<h1>Hello from My First Middleware</h1>");
                }

                // call next middleware func
                //await next.Invoke(env);

                // outbound
            };

            return func;

        }

        public AppFunc MyOtherMiddleware(AppFunc next)
        {
            AppFunc func = async env =>
            {
                // inbound
                var response = env["owin.ResponseBody"] as Stream;
                using (var writer = new StreamWriter(response))
                {
                    await writer.WriteAsync("<h1>Hello from My 2nd Middleware</h1>");
                }

                // call next middleware func
                await next.Invoke(env);

                // outbound
            };

            return func;

        }
    }
}
