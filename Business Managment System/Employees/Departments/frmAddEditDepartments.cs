using Project_Business_Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projects_Managment_System
{
    public partial class frmAddEditDepartments: Form
    {
        private int DepartmentID = -1;
        private Department _Department;
        public enum enMode { AddNew = 1, Update = 2 }
        private enMode _Mode = enMode.AddNew;
        public frmAddEditDepartments()
        {
            InitializeComponent();
            btnClose.CausesValidation = false;
            this.Text = lblTitle.Text = "إضافة قسم";
            _Mode = enMode.AddNew;
            
        }

        public frmAddEditDepartments(int departmentID)
        {
            InitializeComponent();
           
            this.DepartmentID = departmentID;
            this.Text  = lblTitle.Text= "تعديل بيانات القسم";
            this.btnSave.Text = "تعديل";
            
            _Mode = enMode.Update;

        }

        private void txtDepartment_Validating(object sender, CancelEventArgs e)
        {
            if(string.IsNullOrEmpty(txtDepartment.Text))
            {
                e.Cancel = true;
                ep.SetError(txtDepartment, "يجب إدخال أسم القسم");
            }else
                ep.SetError(txtDepartment, null);
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (this.ValidateChildren())
            {
                _Department.DepartmentName = txtDepartment.Text;
                if (_Mode == enMode.AddNew)
                {
                    _Department.CreatedByUserID = 1;
                    // Here will be Active User ID
                   
                }
                else
                {
                    _Department.ModifiedByUserID = 1;
                    // Here will be Active User ID
                  
                }
             
              

                if (await _Department.Save())
                {
                    MessageBox.Show("تم الحفظ بنجاح", "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblDepartmentID.Text = _Department.DepartmentID.ToString();
                    btnClose.Focus();
                    this.Text = lblTitle.Text = "تعديل بيانات القسم";
                    this.btnSave.Text = "تعديل";

                }
                else
                {
                    MessageBox.Show("حدث خطأ أثناء الحفظ", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }

            }
        }

        private async void frmAddEditDepartments_Load_1(object sender, EventArgs e)
        {
            btnClose.CausesValidation = false;
            
            txtDepartment.Focus();

            if (_Mode == enMode.Update)
            {
                _Department = await Task.Run(() => Department.GetDepartment(DepartmentID));

                if (_Department != null)
                {
                    lblDepartmentID.Text = _Department.DepartmentID.ToString();
                    txtDepartment.Text = _Department.DepartmentName;
                    txtDepartment.SelectAll();
                }
                else
                {
                    MessageBox.Show("حدث خطأ أثناء تحميل بيانات القسم", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            else
            {
                _Department = new Department();
                _Department.DepartmentID = await clsglobalSettings.GetNextIDAsync("Departments", "DepartmentId");
                lblDepartmentID.Text = _Department.DepartmentID.ToString();
            }

            
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }
    }
}
