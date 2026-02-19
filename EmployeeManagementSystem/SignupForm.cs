using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class SignupForm : Form
    {
        string conStr = "Host=localhost;Port=5432;Username=postgres;Password=password;Database=employeedb";
        public SignupForm()
        {
            InitializeComponent();
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "" || txtPassword.Text == "")
            {
                MessageBox.Show("All fields are required");
                return;
            }
            
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(conStr))
                {
                    con.Open();

                    string query = "INSERT INTO admin (username, password) VALUES (@name, @password)";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.Parameters.AddWithValue("@password", txtPassword.Text);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Signup Successful!");

                    this.Close();   
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Error" + ex.Message);
            }
        }

        private void SignupForm_Load(object sender, EventArgs e)
        {

        }
    }
}
