using Project_Business_Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projects_Managment_System
{
    public partial class frmDepartments: Form
    {
       private  DataTable _dtAllDepartments;
      
        public frmDepartments()
        {
            InitializeComponent();
           
        }

       
        private async void frmDepartments_Load(object sender, EventArgs e)
        {

            clsglobalSettings.AdjustGridDesign(dgvDepartments);
         
             _dtAllDepartments = await Task.Run(() => Department.GetAllDepartments());
            dgvDepartments.DataSource = _dtAllDepartments;
            cbFilter.SelectedIndex = 0;
            

            lblCount.Text =dgvDepartments.Rows.Count.ToString();


            if (dgvDepartments.Rows.Count > 0)
            {

                cbFilter.Visible = true;

                dgvDepartments.Columns[0].HeaderText = "رقم المسلسل";
                dgvDepartments.Columns[0].Width = 330;

                dgvDepartments.Columns[1].HeaderText = "أسم القسم";
                dgvDepartments.Columns[1].Width = 330;

                dgvDepartments.Columns[2].HeaderText = "تاريخ الإنشاء";
                dgvDepartments.Columns[2].Width = 320;
            }
               
        }

   

        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            
                txtFilter.Visible = cbFilter.Text != "الكل";

            if(txtFilter.Visible) txtFilter.Focus();



        }

      

        private void txtFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilter.SelectedIndex == 1)
                e.Handled = !char.IsDigit(e.KeyChar) && !(e.KeyChar == (char)Keys.Back);
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            string FilterColumn = string.Empty;

            switch (cbFilter.Text)
            {
                case "رقم المسلسل":
                    FilterColumn = "DepartmentID";
                    break;
                case "اسم القسم":
                    FilterColumn = "DepartmentName";
                    break;

                default:
                    FilterColumn = "All";
                    break;
            }

            if (txtFilter.Text.Trim() == "" || FilterColumn == "All")
            {
                _dtAllDepartments.DefaultView.RowFilter = "";
                lblCount.Text =  dgvDepartments.Rows.Count.ToString();
                return;
            }


            if (FilterColumn == "DepartmentID")
                _dtAllDepartments.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, txtFilter.Text.Trim());
            else
                _dtAllDepartments.DefaultView.RowFilter = string.Format("[{0}] Like '{1}%'", FilterColumn, txtFilter.Text.Trim());

            lblCount.Text =  dgvDepartments.Rows.Count.ToString();
        }

       
 

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAddEditDepartments frm = new frmAddEditDepartments();
            frm.ShowDialog();
            frmDepartments_Load(null, null);
        }

        private void AddDepartmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddEditDepartments frm = new frmAddEditDepartments();
            frm.ShowDialog();
            frmDepartments_Load(null, null);
        }

        private void UpdateDepartmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddEditDepartments frm = new frmAddEditDepartments(Convert.ToInt32(dgvDepartments.CurrentRow.Cells[0].Value));
            frm.ShowDialog();
            frmDepartments_Load(null, null);
        }

        private  async void حذفالقسمToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("هل انت متأكد انك تريد حذف القسم", "تعطيل القسم", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            Department _Department = await Department.GetDepartment((int)(dgvDepartments.CurrentRow.Cells[0].Value));

            if (_Department != null && await _Department.DeactiveteDepartment())
            {
                MessageBox.Show("تم حذف القسم بنجاح", "تعطيل القسم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                frmDepartments_Load(null, null);
            }
            else
            {
                MessageBox.Show("فشل حذف القسم", "تعطيل القسم", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
