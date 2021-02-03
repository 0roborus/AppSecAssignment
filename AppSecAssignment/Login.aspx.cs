using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace AppSecAssignment
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_Error.Text = "";
            lbl_Test.Text = Request.QueryString["Comment"];
            lbl_Test.ForeColor = Color.Green;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                tbEmail.Text = tbEmail.Text;
                string email = tbEmail.Text.ToString().Trim();
                string pwd = tbPassword.Text.ToString().Trim();
                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(email);
                string dbSalt = getDBSalt(email);
                if (checkStatus(email) == "1")
                {
                    try
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);
                            if (userHash.Equals(dbHash))
                            {
                                Session["LoggedIn"] = tbEmail.Text.Trim();

                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;

                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                Response.Redirect("HomePage.aspx", false);
                            }
                            else
                            {
                                Session["LoginCount"] = Convert.ToInt32(Session["LoginCount"]) + 1;
                                if (Convert.ToInt32(Session["LoginCount"]) >= 3)
                                {
                                    DeactivateLoginAccount(email);
                                }
                                else
                                {
                                    lbl_Error.Text = "Wrong username or password";
                                }
                            }
                        }
                        else
                        {
                            lbl_Error.Text = "Wrong username or password";
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                    finally { }
                }
                else
                {
                    lbl_Error.Text = "Account has been locked out!";
                    btnRecover.Visible = true;

                }
            }
            else
            {
                lbl_Error.Text = "Please complete the form!";
                lbl_Error.ForeColor = Color.Red;
            }
        }

        private string checkStatus(string email)
        {
            string cs = "1";
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select LoginStatus FROM [User] WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["LoginStatus"] != null)
                        {
                            if (reader["LoginStatus"] != DBNull.Value)
                            {
                                cs = reader["LoginStatus"].ToString();
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
            return cs;
        }

        private string DeactivateLoginAccount(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select * from [User] Where Email=@Email;Update [User] set LoginStatus=0 Where Email=@Email;";
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

        protected string getDBHash(string email)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PassHash FROM [User] WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                h = command.ExecuteNonQuery().ToString();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PassHash"] != null)
                        {
                            if (reader["PassHash"] != DBNull.Value)
                            {
                                h = reader["PassHash"].ToString();
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
            return h;
        }
        protected string getDBSalt(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PassSalt FROM [User] WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PassSalt"] != null)
                        {
                            if (reader["PassSalt"] != DBNull.Value)
                            {
                                s = reader["PassSalt"].ToString();
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
            return s;
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

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LcZa0YaAAAAALl1WdrZcsQGO1jRvwitx2oOFzYk" + "&response=" + captchaResponse);

            try
            {
                using (WebResponse Responses = req.GetResponse())
                {
                    using (StreamReader streamRead = new StreamReader(Responses.GetResponseStream()))
                    {
                        string responseJson = streamRead.ReadToEnd();

                        JavaScriptSerializer jss = new JavaScriptSerializer();

                        MyObject jsonObject = jss.Deserialize<MyObject>(responseJson);

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

        protected void btnRecover_Click(object sender, EventArgs e)
        {
            Response.Redirect("Recover.aspx?Email=" + HttpUtility.UrlEncode(HttpUtility.HtmlEncode(tbEmail.Text)), false);
        }
    }
}