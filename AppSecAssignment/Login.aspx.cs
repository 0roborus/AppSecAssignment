using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSecAssignment
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                if (tbEmail.Text.Trim().Equals("u") && tbPassword.Text.Trim().Equals("p"))
                {
                    Session["LoggedIn"] = tbEmail.Text.Trim();

                    //Create a new GUID and save into the session
                    string guid = Guid.NewGuid().ToString();
                    Session["AuthToken"] = guid;

                    //Now create a new cookie with this guid value
                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                    Response.Redirect("HomePage.aspx", false);
                }
                else
                {
                    lbl_Error.Text = "Wrong username or password";
                }
            }
            else
            {
                lbl_Error.Text = "Please complete the captcha";
            }
        }

        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        public bool ValidateCaptcha()
        {
            bool result = true;
            string captchaResponse = Request.Form["g-recaptcha-response"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LcFXuUZAAAAAMEaTm7RX9l1OrS84_4I1TL3ccei &response=" + captchaResponse);

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }

                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }
}