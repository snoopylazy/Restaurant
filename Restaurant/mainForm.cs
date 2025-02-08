using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Restaurant
{
    public partial class mainForm : Form
    {
        private bool isAdmin;
        private string username;

        private Timer timer;
        public mainForm(bool isAdmin, string username)
        {
            InitializeComponent();
            this.isAdmin = isAdmin;
            this.username = username;

            Datetime();
            starttime();

            labeluser.Text = "សូមស្វាគមន៍, " + username;

            InitializeFormVisibility();
        }

        private void InitializeFormVisibility()
        {
            // Control visibility of forms based on user type
            if (isAdmin)
            {
                // Admin can see all forms and buttons
                ShowAllForms();
            }
            else
            {
                // Non-admin can only see the opForm
                ShowOpForm();
            }
        }

        private void ShowAllForms()
        {
            // Show all forms for admin
            opForm1.Visible = true;
            dashboardForm1.Visible = true;
            categoryForm1.Visible = true;
            inventoryForm1.Visible = true;
            customerForm1.Visible = true;
            chnagePasswordUser1.Visible = true; // Ensure this is visible for admin

            // Enable all buttons for admin
            btnDashboard.Enabled = true;
            btnShop.Enabled = true;
            btnInventory.Enabled = true;
            btnCategory.Enabled = true;
            btnCustomer.Enabled = true;
            btnChangePassUser.Enabled = true; // Enable change password button for admin
        }
        private void ShowOpForm()
        {
            // Show only opForm for non-admin
            opForm1.Visible = true;
            dashboardForm1.Visible = false;
            categoryForm1.Visible = false;
            inventoryForm1.Visible = false;
            customerForm1.Visible = false;
            chnagePasswordUser1.Visible = false; // Hide change password form for non-admin

            // Disable all buttons for non-admin
            btnDashboard.Enabled = false;
            btnShop.Enabled = false;
            btnInventory.Enabled = false;
            btnCategory.Enabled = false;
            btnCustomer.Enabled = false;
            btnChangePassUser.Enabled = false; // Disable change password button for non-admin
        }
        private void Close_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Close?", "Close System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Logout?", "Close System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var loginForm = new Form1();
                loginForm.Show();
                this.Hide();
            }
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            dashboardForm1.Visible = true;
            opForm1.Visible = false;
            categoryForm1.Visible = false;
            inventoryForm1.Visible = false;
            customerForm1.Visible = false;
            chnagePasswordUser1.Visible = false; // Hide change password form
        }

        private void btnShop_Click(object sender, EventArgs e)
        {
            opForm1.Visible = true;
            dashboardForm1.Visible = false;
            categoryForm1.Visible = false;
            inventoryForm1.Visible = false;
            customerForm1.Visible = false;
            chnagePasswordUser1.Visible = false; // Hide change password form
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            inventoryForm1.Visible = true;
            opForm1.Visible = false;
            dashboardForm1.Visible = false;
            categoryForm1.Visible = false;
            customerForm1.Visible = false;
            chnagePasswordUser1.Visible = false; // Hide change password form
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            categoryForm1.Visible = true;
            opForm1.Visible = false;
            dashboardForm1.Visible = false;
            inventoryForm1.Visible = false;
            customerForm1.Visible = false;
            chnagePasswordUser1.Visible = false; // Hide change password form
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            customerForm1.Visible = true;
            opForm1.Visible = false;
            dashboardForm1.Visible = false;
            categoryForm1.Visible = false;
            inventoryForm1.Visible = false;
            chnagePasswordUser1.Visible = false; // Hide change password form
        }
        private void btnChangePassUser_Click(object sender, EventArgs e)
        {
            // Show change password form and hide other forms
            chnagePasswordUser1.Visible = true;
            opForm1.Visible = false;
            dashboardForm1.Visible = false;
            categoryForm1.Visible = false;
            inventoryForm1.Visible = false;
            customerForm1.Visible = false;
        }

        private void labeluser_Click(object sender, EventArgs e)
        {

        }
        private void Datetime()
        {
            label3.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        }
        private void starttime()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Datetime();
        }
    }
}
