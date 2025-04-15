using System;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using System.Security.Cryptography;


namespace DataLayer
{
    
    public class clsDepartmentData
    {
        
        public static async Task<bool> AddDepartmentASync(string departmentName, int createdByUserID,int NewID, DateTime createdDate)
        {
          
            return await clsGlobalCrudOperations.InsertAsync("Departments", new Dictionary<string, object>
            {
                { "DepartmentID", NewID },
                { "DepartmentName", departmentName },
                { "CreatedBy", createdByUserID },
                { "CreatedDate",createdDate },
                { "IsActive", true }
            });
        }

        public static async Task<bool> UpdateDepartmentAsync(int departmentID, string departmentName,
                                           int modifiedByUserID, bool isActive,DateTime ModifiedDate)
        {
            return await clsGlobalCrudOperations.UpdateAsync("Departments", new Dictionary<string, object>
            {
                 { "DepartmentName", departmentName },
                { "ModifiedBy", modifiedByUserID },
                { "ModifiedDate",ModifiedDate },
                { "IsActive", isActive },
            }, new Dictionary<string, object> { { "DepartmentId", departmentID } });
        }
        public static async Task<DataTable> GetAllDepartmentsAsync()
        {
            return await clsGlobalCrudOperations.GetAllAsync("Departments", new List<string> {"DepartmentId","DepartmentName",
        "ModifiedDate" }, new Dictionary<string, object> {{ "IsActive", 1 }} );
        }

        
        public static async Task<bool> DeleteDepartmentAsync(int departmentID,int UserID,DateTime ModifiedDate)
        {
            if (await clsGlobalCrudOperations.CanDeleteRecordAsync("Departments","DepartmentId", departmentID))
            {

                return await clsGlobalCrudOperations.UpdateAsync("Departments", new Dictionary<string, object> { { "IsActive", 0 },
                { "ModifiedBy",UserID},{ "ModifiedDate",ModifiedDate} },
                new Dictionary<string, object> { { "DepartmentId", departmentID } });
            }
            else
                return false;
           
        }

        public static async Task<Dictionary<string,object>> GetDepartmentByIDAsync(int departmentID)
        {
            return await clsGlobalCrudOperations.GetSingleAsync("Departments", new Dictionary<string, object>
        {
            { "DepartmentId", departmentID }
        });
        }

        public static async Task<Dictionary<string, object>> GetDepartmentByNameAsync(string DepartmentName)
        {
            return await clsGlobalCrudOperations.GetSingleAsync("Departments", new Dictionary<string, object>
        {
            { "DepartmentName", DepartmentName }
        });
        }
    }
}