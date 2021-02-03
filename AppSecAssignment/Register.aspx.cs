﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace AppSecAssignment
{
    public partial class Register : System.Web.UI.Page
    {
        string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btCheckPass_Click(object sender, EventArgs e)
        {
            int scores = checkPassword(tbPassword.Text);
            string status = "";
            switch (scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Excellent";
                    break;
                default:
                    break;
            }
            lbl_pwdchecker1.Text = "Status : " + status;
            if (scores < 4)
            {
                lbl_pwdchecker1.ForeColor = Color.Red;
                return;
            }
            lbl_pwdchecker1.ForeColor = Color.Green;
        }
        protected void tbPassword_TextChanged(object sender, EventArgs e)
        {
            int scores = checkPassword(tbPassword.Text);
            string status = "";
            switch (scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Excellent";
                    break;
                default:
                    break;
            }
            lbl_pwdchecker1.Text = "Status : " + status;
            if (scores < 4)
            {
                lbl_pwdchecker1.ForeColor = Color.Red;
                return;
            }
            lbl_pwdchecker1.ForeColor = Color.Green;
        }

        private int checkPassword(string password)
        {
            int score = 0;

            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }

            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[^A-Za-z0-9]"))
            {
                score++;
            }
            tbPassword.Text = password;
            return score;
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (tbFirstName.Text == null || tbLastName.Text == null || tbEmail.Text == null || tbCC.Text == null || tbPassword.Text == null || tbConfirmPassword.Text == null || tbDOB.Text == null || tbPassword.Text != tbConfirmPassword.Text)
            {
                lbl_error.Text = "Please complete the form!";
                lbl_error.ForeColor = Color.Red;
            }
            else
            {
                lbl_error.Text = "Thank you for registering!";
                lbl_error.ForeColor = Color.Green;
                string pwd = tbPassword.Text.ToString().Trim();
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];
                rng.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);
                SHA512Managed hashing = new SHA512Managed();
                string pwdWithSalt = pwd + salt;
                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                finalHash = Convert.ToBase64String(hashWithSalt);
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;
                createAccount();
                Response.Redirect("Login.aspx?Email=" + HttpUtility.UrlEncode(HttpUtility.HtmlEncode(tbEmail.Text)), false);
            }
        }

        public void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO [User] VALUES(@FName, @LName, @CreditCard, @Email, @PasswordHash, @PasswordSalt, @DOB, 1)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@FName", tbFirstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@LName", tbLastName.Text.Trim());
                            cmd.Parameters.AddWithValue("@CreditCard", encryptData(tbCC.Text.Trim()));
                            cmd.Parameters.AddWithValue("@Email", tbEmail.Text.Trim());
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@DOB", tbDOB.Text);                
                            cmd.Connection = con;   
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
               plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }
    }
}