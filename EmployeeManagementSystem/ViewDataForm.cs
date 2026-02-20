using Npgsql;
using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace EmployeeManagementSystem
{
    public partial class ViewDataForm : Form
    {
        string conStr = "Host=localhost;Port=5432;Username=postgres;Password=password;Database=employeedb";
        private int selectedEmpId = -1; 
        public ViewDataForm()
        {
            InitializeComponent();
        }

        private void ViewDataForm_Load(object sender, EventArgs e)
        {
            LoadEmployees();
            AddEditButton();
            dataGridViewEmployees.AutoGenerateColumns = false;

        }
        
        private void LoadEmployees()
        {
            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(conStr))
                {
                    con.Open();
                    string query = "SELECT empid, name, age, salary, department, registration_no FROM employees";
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewEmployees.DataSource = dt;
                }
                AddEditButton();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Add Edit Button Column
        private void AddEditButton()
        {
            if (!dataGridViewEmployees.Columns.Contains("btnEdit"))
            {
                DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn();
                btnEdit.HeaderText = "Action";
                btnEdit.Name = "btnEdit";
                btnEdit.Text = "Edit";
                btnEdit.UseColumnTextForButtonValue = true;
                btnEdit.Width = 60;
                dataGridViewEmployees.Columns.Add(btnEdit);
            }
            // Add Delete Button Column
            if (!dataGridViewEmployees.Columns.Contains("btnDelete"))
            {
                DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();
                btnDelete.HeaderText = "Action";
                btnDelete.Name = "btnDelete";
                btnDelete.Text = "Delete";
                btnDelete.UseColumnTextForButtonValue = true;
                btnDelete.Width = 60;
                dataGridViewEmployees.Columns.Add(btnDelete);
            }
        }


        private void dataGridViewEmployees_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridViewEmployees.Rows[e.RowIndex];

            if (dataGridViewEmployees.Columns[e.ColumnIndex].Name == "btnEdit")
            {
                // Open EmployeeForm
                EmployeeForm empForm = new EmployeeForm();


                // Fill TextBoxes
                empForm.EmployeeName = row.Cells["colName"].Value.ToString();
                empForm.EmployeeAge = row.Cells["colAge"].Value.ToString();
                empForm.EmployeeDepartment = row.Cells["colDepartment"].Value.ToString();
                empForm.EmployeeSalary = row.Cells["colSalary"].Value.ToString();
                empForm.EmployeeRegistrationNo = row.Cells["colregistration_no"].Value.ToString();

                // Store selected empid in a public property
                empForm.SelectedEmpId = Convert.ToInt32(row.Cells["colid"].Value);

                empForm.Show();
                this.Hide();
            }

            // Delete Button
            if (dataGridViewEmployees.Columns[e.ColumnIndex].Name == "btnDelete")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    int empIdToDelete = Convert.ToInt32(row.Cells["colid"].Value);

                    try
                    {
                        using (var con = new NpgsqlConnection(conStr))
                        {
                            con.Open();
                            string query = "DELETE FROM employees WHERE empid=@id";
                            using (var cmd = new NpgsqlCommand(query, con))
                            {
                                cmd.Parameters.AddWithValue("@id", empIdToDelete);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("Record deleted successfully!");
                        LoadEmployees();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting record: " + ex.Message);
                    }
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            EmployeeForm emp = new EmployeeForm();
            emp.Show();
            this.Hide();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = txtSearch.Text.Trim();

              
                if (string.IsNullOrEmpty(searchText))
                {
                    LoadEmployees();
                    return;
                }

                using (NpgsqlConnection con = new NpgsqlConnection(conStr))
                {
                    string query = @"SELECT * FROM employees
                             WHERE name ILIKE @search
                             OR department ILIKE @search
                             OR empid::text ILIKE @search";

                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, con))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@search", "%" + searchText + "%");

                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridViewEmployees.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void btnExportCSV_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.FileName = "Employees.csv";
                if (sfd.ShowDialog() == DialogResult.OK)


                {
                    StringBuilder sb = new StringBuilder();
                    foreach (DataGridViewColumn column in dataGridViewEmployees.Columns)
                    {
                        if (column.Visible)
                        {
                            sb.Append("\"" + column.HeaderText + "\",");
                        }
                    }
                    sb.AppendLine();

                    foreach (DataGridViewRow row in dataGridViewEmployees.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            foreach (DataGridViewColumn column in dataGridViewEmployees.Columns)
                            {
                                if (column.Visible)
                                {
                                    sb.Append("\"" + row.Cells[column.Index].Value?.ToString() + "\",");
                                }
                            }
                            sb.AppendLine();
                        }
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Data exported successfully!");
                }
            }
            catch (Exception ex) 
            {
            MessageBox.Show("Export failed:" + ex.Message);
            }
        }
    }
}
