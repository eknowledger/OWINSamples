using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace MockAuthentication
{
    public static class MockAuthenticationExt
    {
        public static void UseMockAuthentication(this IAppBuilder app)
        {
            app.Use<MockAuthentication>();
        }
    }

}
