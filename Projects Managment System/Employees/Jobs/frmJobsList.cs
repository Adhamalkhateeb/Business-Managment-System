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
    public partial class frmJobsList: Form
    {

        private DataTable _dtAllJobs;
        public frmJobsList()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void  frmJobsList_Load(object sender, EventArgs e)
        {
            clsglobalSettings.AdjustGridDesign(dgvJobs);

            _dtAllJobs = await Task.Run(() => Job.GetAllJobs());
            dgvJobs.DataSource = _dtAllJobs;
            cbFilter.SelectedIndex = 0;


            lblCount.Text = dgvJobs.Rows.Count.ToString();


            if (dgvJobs.Rows.Count > 0)
            {
                cbFilter.Visible = true;

                dgvJobs.Columns[0].HeaderText = "رقم المسلسل";
                dgvJobs.Columns[0].Width = 233;

                dgvJobs.Columns[1].HeaderText = "الوظيفة";
                dgvJobs.Columns[1].Width = 240;

                dgvJobs.Columns[2].HeaderText = "تاريخ التعديل";
                dgvJobs.Columns[2].Width = 230;
            }

         

        }

        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilter.Visible = cbFilter.Text != "الكل";

            if (txtFilter.Visible) txtFilter.Focus();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            string FilterColumn = string.Empty;

            switch (cbFilter.Text)
            {
                case "رقم المسلسل":
                    FilterColumn = "JobID";
                    break;
                case "الوظيفة":
                    FilterColumn = "JobTitle";
                    break;

                default:
                    FilterColumn = "All";
                    break;
            }

            if (txtFilter.Text.Trim() == "" || FilterColumn == "All")
            {
                _dtAllJobs.DefaultView.RowFilter = "";
                lblCount.Text = dgvJobs.Rows.Count.ToString();
                return;
            }


            if (FilterColumn == "JobID")
                _dtAllJobs.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, txtFilter.Text.Trim());
            else
                _dtAllJobs.DefaultView.RowFilter = string.Format("[{0}] Like '{1}%'", FilterColumn, txtFilter.Text.Trim());

            lblCount.Text = dgvJobs.Rows.Count.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilter.SelectedIndex == 1)
                e.Handled = !char.IsDigit(e.KeyChar) && !(e.KeyChar == (char)Keys.Back);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
         frmAdd_Edit_Job frmAdd_Edit_Job = new frmAdd_Edit_Job();
            frmAdd_Edit_Job.ShowDialog();
            frmJobsList_Load(null, null);
           
        }

        private void AddDepartmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAdd_Edit_Job frmAdd_Edit_Job = new frmAdd_Edit_Job();
            frmAdd_Edit_Job.ShowDialog();
        }

        private async void حذفالقسمToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("هل انت متأكد انك تريد حذف الوظيفة ", "حذف الوظيفة ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            Job JOB = await Job.GetJob((int)(dgvJobs.CurrentRow.Cells[0].Value));

            if (JOB != null && await (JOB.DeactiveteJob()))
            {
                MessageBox.Show("تم حذف  الوظيفة بنجاح", "حذف الوظيفة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                frmJobsList_Load(null, null);
            }
            else
            {
                MessageBox.Show("فشل  حذف الوظيفة", "حذف الوظيفة", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void UpdateJobToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAdd_Edit_Job frmAdd_Edit_Job = new frmAdd_Edit_Job((int)dgvJobs.CurrentRow.Cells[0].Value);
            frmAdd_Edit_Job.ShowDialog();
        }

        private void عرضالوظيفةToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAdd_Edit_Job frmAdd_Edit_Job = new frmAdd_Edit_Job((int)dgvJobs.CurrentRow.Cells[0].Value,true);
            frmAdd_Edit_Job.ShowDialog();
        }
    }
}
