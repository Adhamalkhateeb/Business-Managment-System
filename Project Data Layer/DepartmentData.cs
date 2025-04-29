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

        public static async Task<int> AddDepartmentASync(string departmentName, string Description, int CreatedBy)
        {
           return  await clsGlobalDatabase.ExecuteScalarAsync("SP_AddNewDepartment", new SqlParameter[]
           { new SqlParameter("@DepartmentName", departmentName),
             new SqlParameter("@Description", Description) , 
             new SqlParameter("@CreatedBy",CreatedBy) });
        }

        public static async Task<bool> UpdateDepartmentAsync(int departmentID, string Description ,string departmentName, int? modifiedByUserID)
        {
            return await clsGlobalDatabase.ExecuteScalarAsync("SP_UpdateDepartment", new SqlParameter[]
            { new SqlParameter("@DepartmentName", departmentName),
              new SqlParameter("@DepartmentID", departmentID),
              new SqlParameter("@Description", Description) ,
              new SqlParameter("@UpdatedBy",modifiedByUserID) }) != -1;
        }
           
        public static async Task<DataTable> GetAllDepartmentsAsync() =>
            await clsGlobalDatabase.GetAllAsyncByStoredProcedure("SP_GetAllDepartments");



        public static async Task<bool> DeleteDepartmentAsync(int departmentID,int? UserID) =>
            await clsGlobalDatabase.ExecuteScalarAsync("SP_DeleteDepartment", new SqlParameter[]
            { new SqlParameter("@DepartmentID", departmentID),
              new SqlParameter("@UpdatedBy", UserID) }) != -1;


        public static async Task<Dictionary<string, object>> GetDepartmentByIDAsync(int departmentID) =>
             await clsGlobalDatabase.GetSingleRecord("sp_Get_Department", new SqlParameter[]
             {
                 new SqlParameter("@DepartmentID",departmentID)
             });



        public static async Task<Dictionary<string, object>> GetDepartmentByNameAsync(string DepartmentName) =>
              await clsGlobalDatabase.GetSingleRecord("sp_Get_Department_ByName", new SqlParameter[]
              {
                 new SqlParameter("@DepartmentName",DepartmentName)
              });
 

    }
}