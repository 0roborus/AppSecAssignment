using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AppSecAssignment
{
    public partial class ChangePass : System.Web.UI.Page
    {
        string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        private string newFinalHash;
        private string newSalt;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Email"] == null)
            {
                Response.Redirect("Login.aspx", false);
            }
            else
            {
                string email = Session["Email"].ToString();
                int time = checkTime(email);
                if (time < 5)
                {
                    Response.Redirect("Homepage.aspx?Comment=" + HttpUtility.UrlEncode(HttpUtility.HtmlEncode("Too soon to change password! There should be 5 minutes between changes!")), false);
                }
                lbl_message.Text = HttpUtility.HtmlEncode(Request.QueryString["Comment"]);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (tbNPass.Text == null || tbCNPass.Text == null || tbNPass.Text != tbCNPass.Text)
            {
                lbl_error.Text = "Please complete the form!";
                lbl_error.ForeColor = Color.Red;
            }
            else
            {
                string email = Session["Email"].ToString();
                string pwd = tbCPass.Text.ToString().Trim();
                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(email);
                string dbSalt = getDBSalt(email);
                try
                {
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        string pwdWithSalt = pwd + dbSalt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                        string userHash = Convert.ToBase64String(hashWithSalt);
                        if (userHash.Equals(dbHash))
                        {
                            string Npwd = tbNPass.Text.ToString().Trim();
                            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                            byte[] saltByte = new byte[8];
                            rng.GetBytes(saltByte);
                            newSalt = Convert.ToBase64String(saltByte);
                            string NpwdWithSalt = Npwd + newSalt;
                            byte[] NhashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(NpwdWithSalt));
                            newFinalHash = Convert.ToBase64String(NhashWithSalt);
                            if (checkAge(email, newFinalHash))
                            {
                                Response.Redirect("ChangePass.aspx?Comment=" + HttpUtility.UrlEncode(HttpUtility.HtmlEncode("Password has been used recently!")), false);
                            }
                            else
                            {
                                updateAge(email, newFinalHash);
                                ChangePassword(email);
                                Response.Redirect("Login.aspx?Comment=" + HttpUtility.UrlEncode(HttpUtility.HtmlEncode("Password Updated!")), false);
                            }
                        }
                        else
                        {

                            lbl_error.Text = "Wrong current password";

                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally { }

            }
        }

        private string ChangePassword(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "Update [User] set PassHash=@PasswordHash, PassSalt=@PasswordSalt, Time=@time Where Email=@Email;";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@PasswordHash", newFinalHash);
            command.Parameters.AddWithValue("@PasswordSalt", newSalt);
            command.Parameters.AddWithValue("@Time", DateTime.Now);
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

        protected bool checkAge(string email, string newP)
        {
            List<string> age = getDBAge(email).ToString().Split(',').ToList();
            for (var x = 0; x < age.Count; x++)
            {
                if (newP == age[x])
                {
                    return true;
                }
            }
            return false;
        }

        protected bool updateAge(string email, string newP)
        {
            List<string> age = getDBAge(email).ToString().Split(',').ToList();
            if(age.Count == 2)
            {
                age.RemoveAt(0);
                age.Add(newP);
            }
            else
            {
                age.Add(newP);
            }
            return true;
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

        protected string getDBAge(string email)
        {
            string Age = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Age FROM [User] WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Age"] != null)
                        {
                            if (reader["Age"] != DBNull.Value)
                            {
                                Age = reader["Age"].ToString();
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
            return Age;
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