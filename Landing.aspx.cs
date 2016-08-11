using ArdyssLife.PaginaBase;
using Data.Clases;
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
    public partial class Landing : BasePage
    {
        #region Members
        private static BackOfficeV2.Clases.vwPedido _PedidoSession
        {
            get
            {
                return (BackOfficeV2.Clases.vwPedido)HttpContext.Current.Session["_PedidoSession"];
            }
            set
            {
                HttpContext.Current.Session["_PedidoSession"] = value;
            }
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (Request.QueryString["CleanCart"].TieneValor())
                {
                    WebTransactions.RemoveAllViewCart();
                }

                if (!IsPostBack)
                {
                    IsCustomerLogin = false;

                    if (CurrentCountry.TieneValor())
                    {
                        Master.MasterddlCountry.SelectedValue = CurrentCountry.Id.ToString();
                    }

                    if (CurrentLanguage.TieneValor() && CurrentLanguage.Languageid == 1)
                    {

                        panelSpa.Visible = true;
                        PanelIng.Visible = false;

                        if (CurrentCountry.TieneValor())
                        {
                            if (CurrentCountry.Id == 5 || CurrentCountry.Id == 7 || CurrentCountry.Id == 8)
                            {
                                LiBanner72HoursEsp.Visible = false;
                            }
                        }

                    }
                    else
                    {
                        PanelIng.Visible = true;
                        panelSpa.Visible = false;

                        if (CurrentCountry.TieneValor())
                        {
                            if (CurrentCountry.Id == 5 || CurrentCountry.Id == 7 || CurrentCountry.Id == 8)
                            {
                                LiBanner72HoursIng.Visible = false;
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static string EncodePasswordToBase64(string customer_ID)
        {
            try
            {
                byte[] encData_byte = new byte[customer_ID.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(customer_ID);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }


        protected void sendEmail(String email, String nombre, String Message)
        {


            try
            {
                MailMessage msg = new MailMessage();
                //destinatario
                if (CurrentCustomer.Email.TieneValor() && CurrentCustomer.Email != "")
                {
                    msg.To.Add(CurrentCustomer.Email.Trim());
                }
                else
                {
                    msg.To.Add(ConfigurationManager.AppSettings["SMTP.USER"]);
                }

                //quien lo envia
                if (email.Trim().Length > 0)
                {
                    msg.From = new MailAddress(email);
                }

                var request = HttpContext.Current.Request;
                var appUrl = HttpRuntime.AppDomainAppVirtualPath;

                if (appUrl != "/") appUrl += "/";
                var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

                //asunto
                msg.Subject = CurrentLanguage.Languageid == 1 ? "Contacto ArdyssLife" : "ArdyssLife Contact";
                msg.IsBodyHtml = true;
                msg.Body = CurrentLanguage.Languageid == 1 ? "De:" + nombre + "\n Correo:" + email + "\n" + Message : "Of:" + nombre + "\n Correo:" + email + "\n" + Message;

                var clienteSmtp = new SmtpClient
                {
                    UseDefaultCredentials = false,
                    Host = ConfigurationManager.AppSettings["SMTP.HOST"],
                    Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTP.PORT"]),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(new MailAddress(ConfigurationManager.AppSettings["SMTP.USER"], "Ardyss International").Address, ConfigurationManager.AppSettings["SMTP.PASSWORD"]),
                    Timeout = 20000
                };

                clienteSmtp.Send(msg);
                clienteSmtp.Dispose();
                msg.Dispose();

            }
            catch (SmtpException ex)
            {
                throw new SmtpException(ex.Message);
            }
            catch (Exception ex)
            {
                MultiView1.ActiveViewIndex = 2;
                lbMessagge.Text = "" + ex.Message;
            }


        }

        protected void ButtonSend_Click(object sender, EventArgs e)
        {
            try
            {
                sendEmail(TextBoxEmail.Text, TextBoxName.Text, TextBoxMessage.Text);
                MultiView1.ActiveViewIndex = 1;
            }
            catch (Exception ex)
            {
                MultiView1.ActiveViewIndex = 2;
                lbMessagge.Text = "" + ex.Message;
            }
        }

        protected void ButtonCloseMessagge_Click(object sender, EventArgs e)
        {
            try
            {
                MultiView1.ActiveViewIndex = 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}