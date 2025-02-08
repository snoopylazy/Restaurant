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
    public partial class signupForm : Form
    {
        string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Documents\restaurantsystem.mdf;Integrated Security=True;Connect Timeout=30";
        public signupForm()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Close?", "Close System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            var loginForm = new Form1();
            loginForm.Show();
            this.Hide();
        }

        private void showpass_CheckedChanged(object sender, EventArgs e)
        {
            txtpassword.PasswordChar = showpass.Checked ? '\0' : '*';
            txtconfirmpass.PasswordChar = showpass.Checked ? '\0' : '*';
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (SqlConnection connect = new SqlConnection(connection))
            {
                connect.Open();
                string checkUsername = "SELECT * FROM users WHERE username = @usern";

                using (SqlCommand checkUsern = new SqlCommand(checkUsername,connect))
                {
                    checkUsern.Parameters.AddWithValue("@usern", txtusername.Text.Trim());

                    SqlDataAdapter adapter = new SqlDataAdapter(checkUsern);
                    DataTable table = new DataTable();

                    adapter.Fill(table);

                    if(table.Rows.Count != 0)
                    {
                        MessageBox.Show($"{txtusername.Text.Trim()} was taken alreadt","Error Messages", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if(txtpassword.Text.Trim().Length < 8)
                    {
                        MessageBox.Show("Invalid Password, at least 8 characters","Error Messages", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }else if(txtpassword.Text.Trim() != txtconfirmpass.Text.Trim())
                    {
                        MessageBox.Show("Password does not match", "Error Messages", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        string insertData = "INSERT INTO users (username, password, status, date_created) VALUES (@usern, @pass, @status, @date)";
                        using (SqlCommand cmd = new SqlCommand(insertData, connect))
                        {
                            cmd.Parameters.AddWithValue("@usern", txtusername.Text.Trim());
                            cmd.Parameters.AddWithValue("@pass", txtpassword.Text.Trim());
                            cmd.Parameters.AddWithValue("@status", "Active");

                            DateTime today = DateTime.Now;

                            cmd.Parameters.AddWithValue("@date", today);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Registration Successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            var loginForm = new Form1();
                            loginForm.Show();
                            this.Hide();
                        }
                    }
                }
            }
        }

        //private void txtconfirmpass_KeyDown(object sender, KeyEventArgs e)
        //{
        //    btnRegister.Focus();
        //}

        //private void signupForm_KeyDown(object sender, KeyEventArgs e)
        //{

        //}
    }
}
