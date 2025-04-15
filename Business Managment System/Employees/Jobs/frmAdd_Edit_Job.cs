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
    public partial class frmAdd_Edit_Job: Form
    {
        private int _jobID = -1;
        private Job _job;

        public enum enMode { AddNew = 1, Update = 2,Show = 3 }
        private enMode _Mode = enMode.AddNew;
        public frmAdd_Edit_Job()
        {
            InitializeComponent();

            btnClose.CausesValidation = false;
            this.Text = lblTitle.Text = "إضافة وظيفة";
            _Mode = enMode.AddNew;
        }

        public frmAdd_Edit_Job(int jobID, bool Show = false)
        {
            InitializeComponent();

            _jobID = jobID;
            btnClose.CausesValidation = false;
            if (!Show)
            {
                this.Text = lblTitle.Text = "تعديل وظيفة";
                _Mode = enMode.Update;
            }
            else
            {
                this.Text = lblTitle.Text = "بينات الوظيفة";
                _Mode = enMode.Show;
                btnSave.Visible = false;
                btnClose.Location = new Point(btnSave.Location.X, btnClose.Location.Y);
                txtJob.Enabled = false;
                cbDeaprtments.Enabled = false;
            }
        }

        private async void LoadDepartments()
        {
            var departments = await Department.GetAllDepartments();
            foreach (DataRow row in departments.Rows)
            {
                cbDeaprtments.Items.Add(row["DepartmentName"].ToString());
            }
        }

        private async void frmAdd_Edit_Job_Load(object sender, EventArgs e)
        {
            btnClose.CausesValidation = false;
            LoadDepartments();
            txtJob.Focus();

        

            if (_Mode == enMode.Update || _Mode == enMode.Show)
            {
                _job = await Task.Run(() => Job.GetJob(_jobID));
                if(_job == null)
                {
                    MessageBox.Show("لا توجد بيانات للوظيفة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
                lblJobID.Text = _job.JobID.ToString();
                txtJob.Text = _job.JobTitle;
                txtJob.SelectAll();
                cbDeaprtments.SelectedIndex = cbDeaprtments.FindString(_job.Department.DepartmentName);
            }
            else
            {
                _job = new Job();
                _job.JobID = await clsglobalSettings.GetNextIDAsync("Job", "JobID");
                lblJobID.Text = _job.JobID.ToString();

            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void txtDepartment_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtJob.Text))
            {
                e.Cancel = true;
                ep.SetError(txtJob, "يجب إدخال الوظيفة اولا");
            }
            else
                ep.SetError(txtJob, null);
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (this.ValidateChildren())
            {
                _job.JobTitle = txtJob.Text;
                if (_Mode == enMode.AddNew)
                {
                    _job.CreatedBy = 1;
                    // Here will be Active User ID

                }
                else
                {
                    _job.ModifiedBy = 1;
                    // Here will be Active User ID

                }
                _job.DepartmentID = (await Department.GetDepartment(cbDeaprtments.Text)).DepartmentID;

                if (await _job.Save())
                {
                    MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblJobID.Text = _job.JobID.ToString();
                    btnClose.Focus();
                    this.Text = lblTitle.Text = "تعديل الوظيفة";
                    this.btnSave.Text = "تعديل";

                }
                else
                {
                    MessageBox.Show("حدث خطأ أثناء الحفظ", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }

            }
        }

        private void cbDeaprtments_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(cbDeaprtments.Text))
            {
                e.Cancel = true;
                ep.SetError(cbDeaprtments, "يجب إختيار القسم اولا");
            }
            else
                ep.SetError(cbDeaprtments, null);
        }
    }
}
