using ArdyssLife.PaginaBase;
using BackOfficeV2.Clases;
using Data.Clases;
using Data.ExigoApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ArdyssLife
{
    public partial class PageTest : System.Web.UI.Page
    {
        private static ApiAuthentication auth = new ApiAuthentication()
        {
            LoginName = "APIBackOffice",
            Password = "Ardy55#2011",
            Company = "ardyss"
        };
        private static ExigoApiSoapClient api = new ExigoApiSoapClient();
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                BasePage.sendEmail("omartinez@ardyss.com", "omartinez@ardyss.com",78765314,"Oscar Test","26007895","789456","120.00","2016-05-05","3317144464","3336290464");
            }
            catch (Exception)
            {
                
                throw;
            }
        }

    }
}