using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsApplication2
{
    public partial class ProgressFrom : Form
    {
        public ProgressFrom()
        {
            InitializeComponent();
        }

        public ProgressFrom(string val)
        {
            InitializeComponent();
            this.ControlBox = false;
            this.Text = val;
        }

        //去掉右上角的大叉
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                const int CS_NOCLOSE = 0x200;
                cp.ClassStyle = cp.ClassStyle | CS_NOCLOSE;
                return cp;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
