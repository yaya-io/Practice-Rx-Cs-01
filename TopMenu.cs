﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Practice_Rx_Cs_01
{
    public partial class TopMenu : Form
    {
        public TopMenu()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            new Form1().Show();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            new Form2().Show();
        }
    }
}
