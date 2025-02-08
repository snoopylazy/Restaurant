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
            LoadUsers();
            dataGridView1.CellClick += DataGridView1_CellClick;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedUsername = row.Cells["username"].Value.ToString();
                textusername.Text = selectedUsername;
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

            // If the checkbox is checked, hash the password; otherwise, store it as plain text
            string finalPassword = checkBox1.Checked ? HashPassword(newPassword) : newPassword;

            UpdatePassword(selectedUsername, finalPassword);

            MessageBox.Show("Password changed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                    string storedPassword = result.ToString().Trim();
                    // If checkbox is checked, compare hashed passwords
                    if (checkBox1.Checked)
                        return storedPassword == HashPassword(enteredPassword);
                    else
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
            LoadUsers();
        }

        private void LoadUsers(string search = "")
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT id, username, password, status, date_created FROM users";
                if (!string.IsNullOrEmpty(search))
                {
                    query += " WHERE username LIKE @search";
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
            textusername.Clear();
            txtpassword.Clear();
            txtnewpassword.Clear();
            txtConfirmPass.Clear();
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            LoadUsers(textSearch.Text.Trim());
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedUsername))
            {
                MessageBox.Show("Please select a user from the list!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show($"Are you sure you want to delete the user '{selectedUsername}'?",
                                                  "Confirm Delete",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string query = "DELETE FROM users WHERE username = @username";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", selectedUsername);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("User deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                selectedUsername = ""; // Clear selection
                textusername.Clear();   // Clear username field
                LoadUsers();            // Refresh the user list
            }
        }
        private void btnCreate_Click(object sender, EventArgs e)
        {
            string username = textusername.Text.Trim();
            string password = txtnewpassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Username and password are required!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                // Check if the username already exists
                string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@username", username);
                int userExists = (int)checkCmd.ExecuteScalar();

                if (userExists > 0)
                {
                    MessageBox.Show("This username is already taken. Please choose another one!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Insert the new user
                string finalPassword = checkBox1.Checked ? HashPassword(password) : password;
                string query = "INSERT INTO users (username, password, status, date_created) VALUES (@username, @password, 'Active', GETDATE())";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", finalPassword);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("User created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadUsers();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //bool isChecked = checkBox1.Checked;
            //txtpassword.UseSystemPasswordChar = !isChecked;
            //txtnewpassword.UseSystemPasswordChar = !isChecked;
            //txtConfirmPass.UseSystemPasswordChar = !isChecked;
        }

        private void chnagePasswordUser_Load(object sender, EventArgs e)
        {

        }
    }
}
