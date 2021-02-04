using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSecAssignment
{
    public partial class HomePage : System.Web.UI.Page
    {
        string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Email"] == null)
            {
                Response.Redirect("Login.aspx", false);
            }
            else
            {
                string email = Session["Email"].ToString();
                var time = checkTime(email);
                if (time > 15)
                {
                    Response.Redirect("ChangePass.aspx?Comment=" + HttpUtility.UrlEncode(HttpUtility.HtmlEncode("Your password is too old! Please change your password!")), false);
                }
                if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
                {
                    if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                    {
                        Response.Redirect("Login.aspx", false);
                    }
                    else
                    {
                        Session["LoginCount"] = 0;
                        if (HttpUtility.HtmlEncode(Request.QueryString["Comment"]) != null)
                        {
                            lblMessage.Text = HttpUtility.HtmlEncode(Request.QueryString["Comment"]);
                        }
                        else
                        {
                            lblMessage.Text = "Congratulations! , you are logged in.";
                            lblMessage.ForeColor = System.Drawing.Color.Green;
                        }
                        btnLogout.Visible = true;
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx", false);
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Request.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Request.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }
            if (Request.Cookies["AuthToken"] != null)
            {
                Request.Cookies["AuthToken"].Value = string.Empty;
                Request.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }

        protected void btnChangePass_Click(object sender, EventArgs e)
        {
            Response.Redirect("ChangePass.aspx?Email=" + HttpUtility.UrlEncode(HttpUtility.HtmlEncode(Request.QueryString["Email"])), false);
        }

        protected int checkTime(string email)
        {
            TimeSpan time = new TimeSpan();
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Time FROM [User] WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Time"] != null)
                        {
                            if (reader["Time"] != DBNull.Value)
                            {
                                DateTime old = Convert.ToDateTime(reader["Time"].ToString());
                                time = DateTime.Now.Subtract(old);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return time.Minutes;
        }
    }
}