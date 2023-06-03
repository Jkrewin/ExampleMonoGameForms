using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonoGame.Forms.DX
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }





        private void Form1_Load(object sender, EventArgs e)
        {
            sampleControl.VisibleUI_Form1 += UI_vis;
        }

        private void Button1_Click(object sender, EventArgs e)        {
           
            sampleControl.RedBlock_Visible = !sampleControl.RedBlock_Visible;
        }

     

        private void UI_vis(bool vis) {

            button1.Visible = vis;
            label1.Visible = vis;
        }


        
    }
}
