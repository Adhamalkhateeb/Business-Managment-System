
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
    public partial class frmMain :Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private  void SearchDepartmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDepartments frm = new frmDepartments();
               frm.Show();
                
            
        }

        private void AddDepartmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
           frmAddEditDepartments frm = new frmAddEditDepartments();
            frm.ShowDialog();
        }

        private void بحثعنوظيفةToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmJobsList frm = new frmJobsList();
            frm.Show();
        }

        private void اضافةوظيفةToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAdd_Edit_Job frmAdd_Edit_Job = new frmAdd_Edit_Job();
            frmAdd_Edit_Job.ShowDialog();
        }

        private void EmployeesToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }
    }
}
