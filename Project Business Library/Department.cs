
using DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project_Business_Library
{
    public class  Department
    {
        public enum enMode { AddNew = 1, Update = 2 }
        private enMode _Mode = enMode.AddNew;
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public Department()
        {
            DepartmentID = -1;
            DepartmentName = string.Empty;
            Description = null;
            CreatedByUserID = -1;
            ModifiedByUserID = null;
            CreatedDate =DateTime.Now;
            ModifiedDate = null;
            IsActive = true;

            _Mode = enMode.AddNew;

        }

        private Department(int departmentID, string departmentName,string Description,int createdByUserID, DateTime createdDate, int? modifiedByUserID, DateTime? modifiedDate, bool IsActive)
        {
            this.DepartmentID = departmentID;
            this.DepartmentName = departmentName;
            this.Description = Description;
            this.CreatedByUserID = createdByUserID;
            this.CreatedDate = createdDate;
            this.ModifiedByUserID = modifiedByUserID;
            this.ModifiedDate = modifiedDate;
            this.IsActive = IsActive;

            _Mode = enMode.Update;
        }

        static public async Task<DataTable> GetAllDepartments()
        {
           return await clsDepartmentData.GetAllDepartmentsAsync();
        }

        static public async Task<Department> GetDepartment(int departmentID)
        {
             Dictionary<string,object> result = await clsDepartmentData.GetDepartmentByIDAsync(departmentID);

            if(result != null)
            {
                return new Department(
                      (int)result["DepartmentID"],
                      result["DepartmentName"].ToString(),
                      result["Description"] == null ? null : result["Description"].ToString(),
                      (int)result["CreatedBy"],
                      (DateTime)result["CreationDate"],
                      result["UpdatedBy"] == null ? (int?)null : (int?)result["UpdatedBy"],
                     result["UpdatedDate"] == null ? (DateTime?)null : (DateTime?)result["UpdatedDate"],
                      (bool)result["IsActive"]) ;                   
            }
            else
            {
                return null;
            }

        }


        static public async Task<Department> GetDepartment(string DepartmentName)
        {
            Dictionary<string, object> result = await clsDepartmentData.GetDepartmentByNameAsync(DepartmentName);

            if (result != null)
            {
                return new Department(
                      (int)result["DepartmentID"],
                      result["DepartmentName"].ToString(),
                      result["Description"] == null ? null : result["Description"].ToString(),
                      (int)result["CreatedBy"],
                      (DateTime)result["CreationDate"],
                      result["UpdatedBy"] == null ? (int?)null : (int?)result["UpdatedBy"],
                     result["UpdatedDate"] == null ? (DateTime?)null : (DateTime?)result["UpdatedDate"],
                      (bool)result["IsActive"]);
            }
            else
            {
                return null;
            }

        }

        private async Task <bool> AddDepartment()
        {
            return  (await clsDepartmentData.AddDepartmentASync(this.DepartmentName,this.Description , this.CreatedByUserID)) != -1;
        }

        private async Task<bool> UpdateDepartment()
        {
            return  await clsDepartmentData.UpdateDepartmentAsync(this.DepartmentID, this.Description,this.DepartmentName,this.ModifiedByUserID);
        }



       public  async Task<bool> DeleteDepartment()
        {
            
            return await clsDepartmentData.DeleteDepartmentAsync(this.DepartmentID,this.ModifiedByUserID);
        }

       

        public  async Task<bool> Save()
        {
            switch(_Mode)
            {
                case enMode.AddNew:
                   if( await AddDepartment())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                   else
                    {
                        return false;
                   }
                case enMode.Update:
                    return await UpdateDepartment();
                default:
                    return false;
            }
        }



        public override string ToString()
        {
            return DepartmentName;
        }
    }
}
