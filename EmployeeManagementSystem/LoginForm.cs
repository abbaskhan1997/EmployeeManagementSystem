using Npgsql;
using System;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class LoginForm : Form
    {
        string conStr = "Host=localhost;Port=5432;Username=postgres;Password=password;Database=employeedb";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(conStr))
                {
                    con.Open();

                    string query = "SELECT COUNT(*) FROM admin WHERE username=@name AND password=@password";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@name", txtUsername.Text);
                        cmd.Parameters.AddWithValue("@password", txtPassword.Text);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count == 1)
                        {
                            MessageBox.Show("Login Successful!");

                            Dashboard dash = new Dashboard();
                            dash.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Invalid Username or Password");
                            txtUsername.Focus();

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error"+ ex.Message);
            }

        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            SignupForm signupForm = new SignupForm();
            signupForm.ShowDialog();
           
        }
    }
}
