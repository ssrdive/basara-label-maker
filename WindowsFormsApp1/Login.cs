using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;


namespace WindowsFormsApp1
{
    public partial class Login : Form
    {
        private static readonly HttpClient client = new HttpClient();

        public Login()
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;

            String username = txtUsername.Text;
            String password = txtPassword.Text;

            var values = new Dictionary<string, string>
            {
                { "username", username },
                {"password", password}
            };

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://farmgear.app/api/authenticate", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if(response.IsSuccessStatusCode)
            {
                dynamic json = JsonConvert.DeserializeObject(responseString);
                string token = json.token;
                this.Hide();
                Form1 form1 = new Form1(token);
                form1.ShowDialog();
            } else
            {
                MessageBox.Show("Invalid credentials. Please try again");
                btnLogin.Enabled = true;
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
