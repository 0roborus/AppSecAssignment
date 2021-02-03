using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSecAssignment
{
    public partial class Recover : System.Web.UI.Page
    {
        string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        private string ActivateLoginAccount(string email)   
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * from [User] Where Email=@Email;Update [User] set LoginStatus=1 Where Email=@Email;";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return null;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (tbVC.Text.ToLower() == Session["CaptchaVerify"].ToString())
            {
                ActivateLoginAccount(Request.QueryString["Email"]);
                string comment = "Account Unlocked!";
                Response.Redirect("Login.aspx?Comment=" + HttpUtility.UrlEncode(HttpUtility.HtmlEncode(comment)));
            }
            else
            {
                lbCaptchaMessage.Text = "You have entered the wrong Captcha. Please enter the correct Captcha!";
                lbCaptchaMessage.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}