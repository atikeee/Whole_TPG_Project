using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Uploading_db
{
    public partial class Form_Confirm : Form
    {
        public Form_Confirm(string st)
        {
            InitializeComponent();
            labelWarning.Text = st;
        }

       // public void updateLabel(string st)
       // {
       //     labelWarning.Text = st;
       // }
    }
}
