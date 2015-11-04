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
        public static void UseMyMiddleware(this IAppBuilder app, MyMiddlewareConfigOptions options)
        {
            app.Use<MyMiddleware>(options);
        }

        public static void UseMyOtherMiddleware(this IAppBuilder app)
        {
            app.Use<MyOtherMiddleware>();
        }

        public static void UseAuthentication(this IAppBuilder app)
        {
            app.Use<MockAuthentication>();
        }

        public static void UseLogging(this IAppBuilder app)
        {
            app.Use<MockLogging>();
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            MyMiddlewareConfigOptions options = new MyMiddlewareConfigOptions("Hawdy", "Ahmed") {IncludeDate = true};
            app.UseLogging();
            app.UseAuthentication();
            app.UseMyMiddleware(options);
            //app.UseMyOtherMiddleware();

        }

    }

    public class MyMiddleware
    {
        private AppFunc _next;
        private readonly MyMiddlewareConfigOptions _options;

        public MyMiddleware(AppFunc next, MyMiddlewareConfigOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {

                // inbound
            IOwinContext context = new OwinContext(env);
            await context.Response.WriteAsync(string.Format("<h1>{0}</h1>", _options.GetGreeting()));

            // Update the response code to 200 OK:
            context.Response.StatusCode = 200;
            context.Response.ReasonPhrase = "OK";

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

    public class MockAuthentication
    {
        private AppFunc _next;
        public MockAuthentication(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            // In the real world we would do REAL auth processing here...

            var isAuthorized = context.Request.QueryString.Value == "ahmed";
            if (!isAuthorized)
            {
                context.Response.StatusCode = 401;
                context.Response.ReasonPhrase = "Not Authorized";

                // Send back a really silly error page:
                await context.Response.WriteAsync(string.Format("<h1>Error {0}-{1}",
                    context.Response.StatusCode,
                    context.Response.ReasonPhrase));
            }
            else
            {
                // _next is only invoked is authentication succeeds:
                context.Response.StatusCode = 200;
                context.Response.ReasonPhrase = "OK";
                await _next.Invoke(environment);
            }
        }

    }


    public class MockLogging
    {
        AppFunc _next;
        public MockLogging(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            // Pass everything up through the pipeline first:
            await _next.Invoke(environment);

            // Do the logging on the way out:
            IOwinContext context = new OwinContext(environment);
            Console.WriteLine("URI: {0} Status Code: {1}",
                context.Request.Uri, context.Response.StatusCode);
        }
    }

    public class MyMiddlewareConfigOptions
    {
        string _greetingTextFormat = "{0} from {1}{2}";
        public MyMiddlewareConfigOptions(string greeting, string greeter)
        {
            GreetingText = greeting;
            Greeter = greeter;
            Date = DateTime.Now;
        }

        public string GreetingText { get; set; }
        public string Greeter { get; set; }
        public DateTime Date { get; set; }

        public bool IncludeDate { get; set; }

        public string GetGreeting()
        {
            string DateText = "";
            if (IncludeDate)
            {
                DateText = string.Format(" on {0}", Date.ToShortDateString());
            }
            return string.Format(_greetingTextFormat, GreetingText, Greeter, DateText);
        }
    }


}
