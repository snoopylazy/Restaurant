using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Restaurant
{
    public partial class Form1 : Form
    {
        string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Documents\restaurantsystem.mdf;Integrated Security=True;Connect Timeout=30";
        public Form1()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to Close?", "Close System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            var signupForm = new signupForm();
            signupForm.Show();
            this.Hide();
        }

        //private void btnLogin_Click(object sender, EventArgs e)
        //{
        //    using (SqlConnection connect = new SqlConnection(connection)) 
        //    { 
        //        connect.Open();
        //        string query = "SELECT * FROM users WHERE username = @usern AND password = @pass";
        //        using (SqlCommand cmd = new SqlCommand(query, connect))
        //        {
        //            cmd.Parameters.AddWithValue("@usern", txtusername.Text.Trim());
        //            cmd.Parameters.AddWithValue("@pass", txtpassword.Text.Trim());

        //            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //            DataTable table = new DataTable();

        //            adapter.Fill(table);

        //            if (table.Rows.Count > 0)
        //            {
        //                MessageBox.Show("Login Successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                var mainForm = new mainForm();
        //                mainForm.Show();
        //                this.Hide();
        //            }
        //            else
        //            {
        //                MessageBox.Show("Login Failed", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            }
        //        }
        //    }
        //}

        private void showpass_CheckedChanged(object sender, EventArgs e)
        {
            txtpassword.PasswordChar = showpass.Checked ? '\0' : '*';
        }
        private void login_Click(object sender, EventArgs e)
        {
            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();
                string query = "SELECT * FROM users WHERE username = @usern AND password = @pass";
                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@usern", txtusername.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", txtpassword.Text.Trim());

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable table = new DataTable();

                    adapter.Fill(table);

                    if (table.Rows.Count > 0)
                    {
                        // Get username from the database
                        string username = table.Rows[0]["username"].ToString();

                        // **Store username in UserSession**
                        UserSession.LoggedInUser = username;

                        // Check if user is admin
                        bool isAdmin = (username == "admin");

                        // Open main form and pass login status
                        var mainForm = new mainForm(isAdmin, username);
                        mainForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Login Failed", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        //private void btnLogin(object sender, EventArgs e)
        //{

        //}
    }
}
