using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Security
{
    public partial class ConfigureWinLogin : Form
    {
        public ConfigureWinLogin(string userName, string domain)
        {
            InitializeComponent();
            tUsername.Text = userName;
            tDomain.Text = domain;
        }

        public string Password
        {
            get { return tPassword.Text; }
        }

        public event EventHandler PasswordSet;

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (PasswordSet != null) PasswordSet(this, EventArgs.Empty);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void tPassword_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = !string.IsNullOrEmpty(tPassword.Text);
        }
    }
}
