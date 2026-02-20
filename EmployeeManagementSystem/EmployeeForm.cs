using Npgsql;
using System;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class EmployeeForm : Form
    {
        private string conStr = "Host=localhost;Port=5432;Username=postgres;Password=password;Database=employeedb";
        private int selectedEmpId = -1;

        public EmployeeForm ()
        {
            InitializeComponent();
        }

        private void EmployeeForm_Load (object sender, EventArgs e)
        {
        }

        public void Clear ()
        {
            txtName.Clear();
            txtAge.Clear();
            txtSalary.Clear();
            txtDepartment.Clear();
            txtRegistrationNo.Clear();
            txtName.Focus();
        }

        // View Button Click
        private void btnView_Click (object sender, EventArgs e)
        {
            ViewDataForm VDF = new ViewDataForm();
            VDF.Show();
            this.Hide();
        }

        // Save Button Click
        private void btnSave_Click (object sender, EventArgs e)
        {
            if (txtName.Text == "" || txtAge.Text == "" || txtSalary.Text == "" || txtDepartment.Text == "" || txtRegistrationNo.Text == "")
            {
                MessageBox.Show("All fields are required");
                return;
            }
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(conStr))
                {
                    con.Open();
                    string query = "INSERT INTO employees (name, age, salary, department,registration_no) VALUES (@name, @age, @salary, @department,@registration_no)";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.Parameters.AddWithValue("@age", int.Parse(txtAge.Text));
                        cmd.Parameters.AddWithValue("@salary", decimal.Parse(txtSalary.Text));
                        cmd.Parameters.AddWithValue("@department", txtDepartment.Text);
                        cmd.Parameters.AddWithValue("@registration_no", txtRegistrationNo.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Employee saved successfully ✅");

                Clear();
            }
            catch (FormatException)
            {
                MessageBox.Show("Age and Salary must contain numbers only");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Back Button Click
        private void btnBack_Click (object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Hide();
        }

        public int SelectedEmpId { get; set; }

        public string EmployeeName
        {
            get { return txtName.Text; }
            set { txtName.Text = value; }
        }

        public string EmployeeAge
        {
            get { return txtAge.Text; }
            set { txtAge.Text = value; }
        }

        public string EmployeeDepartment
        {
            get { return txtDepartment.Text; }
            set { txtDepartment.Text = value; }
        }

        public string EmployeeSalary
        {
            get { return txtSalary.Text; }
            set { txtSalary.Text = value; }
        }

        public string EmployeeRegistrationNo
        {
            get { return txtRegistrationNo.Text; }
            set { txtRegistrationNo.Text = value; }
        }

        private void btnUpdate_Click (object sender, EventArgs e)
        {
            if (SelectedEmpId == 0)
            {
                MessageBox.Show("Select a record first!");
                return;
            }

            try
            {
                using (var con = new NpgsqlConnection(conStr))
                {
                    con.Open();
                    string query = "UPDATE employees SET name=@name, age=@age, department=@department, salary=@salary WHERE empid=@id";
                    using (var cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", SelectedEmpId);
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.Parameters.AddWithValue("@age", int.Parse(txtAge.Text));
                        cmd.Parameters.AddWithValue("@department", txtDepartment.Text);
                        cmd.Parameters.AddWithValue("@salary", decimal.Parse(txtSalary.Text));
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Record updated successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            Clear();
        }
    }
}