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
    public partial class loadingProgress : Form
    {
        private Random random = new Random();
        //private int label5Direction = 5; // Movement speed (positive = right, negative = left)
        public loadingProgress()
        {
            InitializeComponent();
        }

        private void loadingProgress_Load(object sender, EventArgs e)
        {
            timer1.Start();
            timer1.Interval = 50; // Adjust speed (lower = faster)
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < 100)
            {
                progressBar1.Value += 1;
                label3.Text = progressBar1.Value.ToString() + "%";

                // 🌊 **Wave Animation for label1** (Moves Up & Down)
                //int waveY = 20 * (int)Math.Sin(progressBar1.Value * Math.PI / 20);
                //label1.Top = 50 + waveY;

                // 🎨 Auto Color Change for label1 and label5
                label1.ForeColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                //label5.ForeColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));

                // 🏃 **Animate Label5 Left to Right**
                //label5.Left += label5Direction;

                // 🔄 **Bounce Effect on Form Boundaries**
                //if (label5.Left <= 10 || label5.Right >= this.ClientSize.Width - 10)
                //{
                //    label5Direction *= -1; // Reverse direction
                //}
            }
            else
            {
                timer1.Stop();
                Form1 login = new Form1();
                login.Show();
                this.Hide();
            }
        }
    }
}
