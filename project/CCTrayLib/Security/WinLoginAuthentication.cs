using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Security
{
    [Extension(DisplayName="WinLogin authentication")]
    public class WinLoginAuthentication
        : IAuthenticationMode
    {
        private string settings;
        private readonly string userName = Environment.UserName;
        private readonly string domain = Environment.UserDomainName;
        private readonly string rsaContainerName = "CruiseControl.NET WinLogin Settings";

        public string Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public bool Configure(IWin32Window owner)
        {
            ConfigureWinLogin newDialog = new ConfigureWinLogin(domain, userName);
            newDialog.WinLoginSet += delegate(object sender, EventArgs e) { settings = EncryptSettings(newDialog.Password); };
            DialogResult result = newDialog.ShowDialog(owner);
            return (result == DialogResult.OK);
        }

        public bool Validate()
        {
            return !string.IsNullOrEmpty(settings);
        }

        public LoginRequest GenerateCredentials()
        {
            LoginRequest credentials = new LoginRequest(userName);
            credentials.AddCredential(LoginRequest.DomainCredential, domain);
            credentials.AddCredential(LoginRequest.PasswordCredential, DecryptSettings(settings));
            return credentials;
        }

        #region EncryptSettings()
        private string EncryptSettings(string value)
        {
            byte[] plaintext = Encoding.Unicode.GetBytes(value);

            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = rsaContainerName;
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048, cspParams))
            {
                byte[] encryptedData = RSA.Encrypt(plaintext, false);
                return Convert.ToBase64String(encryptedData);
            }
        }
        #endregion

        #region DecryptSettings()
        private string DecryptSettings(string value)
        {
            byte[] encryptedData = Convert.FromBase64String(value);

            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = rsaContainerName;
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048, cspParams))
            {
                byte[] decryptedData = RSA.Decrypt(encryptedData, false);
                return Encoding.Unicode.GetString(decryptedData);
            }
        }
        #endregion
    }
}
