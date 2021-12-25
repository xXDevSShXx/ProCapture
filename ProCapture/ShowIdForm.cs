﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProCapture
{
    public partial class ShowIdForm : Form
    {
        public ShowIdForm(string Id)
        {
            InitializeComponent();
            idTextBox.Text = Id;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            idTextBox.Copy();
        }

        private void ShowIdForm_Load(object sender, EventArgs e)
        {

        }
    }
}
