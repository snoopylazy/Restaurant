using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Restaurant
{
    public partial class chnagePasswordUser : UserControl
    {
        private string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\Documents\restaurantsystem.mdf;Integrated Security=True;Connect Timeout=30";
        private string selectedUsername = ""; // Stores selected user from DataGridView

        public chnagePasswordUser()
        {
            InitializeComponent();
            LoadUsers(); // Load users on startup
            dataGridView1.CellClick += DataGridView1_CellClick; // Handle row click
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ensure it's not the header row
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedUsername = row.Cells["username"].Value.ToString();
                txtusername.Text = selectedUsername;
            }
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            string oldPassword = txtpassword.Text.Trim();
            string newPassword = txtnewpassword.Text.Trim();
            string confirmPassword = txtConfirmPass.Text.Trim();

            if (string.IsNullOrEmpty(selectedUsername))
            {
                MessageBox.Show("Please select a user from the list!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("All fields are required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("New passwords do not match!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!VerifyOldPassword(selectedUsername, oldPassword))
            {
                MessageBox.Show("Old password is incorrect!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //string hashedNewPassword = HashPassword(newPassword);
            //UpdatePassword(selectedUsername, hashedNewPassword);
            UpdatePassword(selectedUsername, newPassword);

            MessageBox.Show("Password changed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Clear fields
            txtpassword.Clear();
            txtnewpassword.Clear();
            txtConfirmPass.Clear();
        }

        private bool VerifyOldPassword(string username, string enteredPassword)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT password FROM users WHERE username=@username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    string storedPassword = result.ToString();

                    // Trim spaces to avoid issues
                    storedPassword = storedPassword.Trim();
                    enteredPassword = enteredPassword.Trim();

                    return storedPassword == enteredPassword;
                }
                return false;
            }
        }

        private void UpdatePassword(string username, string newPasswordHash)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "UPDATE users SET password=@password WHERE username=@username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@password", newPasswordHash);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.ExecuteNonQuery();
            }

            LoadUsers(); // Refresh DataGridView
        }

        private void LoadUsers(string search = "")
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT id, username, password, status, date_created FROM users";

                if (!string.IsNullOrEmpty(search))
                {
                    query += " WHERE id LIKE @search OR username LIKE @search";
                }

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtpassword.Clear();
            txtnewpassword.Clear();
            txtConfirmPass.Clear();
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            LoadUsers(textSearch.Text.Trim());
        }
    }
}
