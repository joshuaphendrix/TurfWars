using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace TurfWars
{
    public class Global : System.Web.HttpApplication
    {
        public static TurfWarsGame game;
        protected void Application_Start(object sender, EventArgs e)
        {
            game = new TurfWarsGame();
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}