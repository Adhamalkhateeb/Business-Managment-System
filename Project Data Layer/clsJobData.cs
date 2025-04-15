using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Data_Layer
{
    public class clsJobData
    {

        static public async Task<DataTable> GetAllJobsAsync()
        {
            return await clsGlobalCrudOperations.GetAllAsync("Job", new string[] { "JobID", "JobTitle","ModifiedDate"},
                new Dictionary<string, object> { { "IsActive", 1 } });
        }

        public static async Task<Dictionary<string, object>> GetJobByIDAsync(int JobID)
        {
            return await clsGlobalCrudOperations.GetSingleAsync("Job", new Dictionary<string, object>
        {
            { "JobID", JobID }
        });
        }

        public static async Task<bool> AddJobASync(int JobID, int createdByUserID, string JobTitle, DateTime createdDate,int DepartmentID)
        {

            return await clsGlobalCrudOperations.InsertAsync("Job", new Dictionary<string, object>
            {
                { "JobID", JobID },
                { "JobTitle", JobTitle },
                { "CreatedBy", createdByUserID },
                { "CreatedDate",createdDate },
                { "IsActive", true },
                {"DepartmentID",DepartmentID }
            });
        }

        public static async Task<bool> UpdateJobAsync(int JobID, string JobTitle,
                                           int modifiedByUserID, bool isActive, DateTime ModifiedDate,int DepartmentID)
        {
            return await clsGlobalCrudOperations.UpdateAsync("Job", new Dictionary<string, object>
            {
                 { "JobTitle", JobTitle },
                { "ModifiedBy", modifiedByUserID },
                { "ModifiedDate",ModifiedDate },
                { "IsActive", isActive },
                {"DepartmentID", DepartmentID   }
            }, new Dictionary<string, object> { { "JobID", JobID } });
        }

        public static async Task<bool> DeleteJobAsync(int JobID, int UserID, DateTime ModifiedDate)
        {
            if (await clsGlobalCrudOperations.CanDeleteRecordAsync("Job", "JobID",JobID))
            {
                return await clsGlobalCrudOperations.UpdateAsync("Job", new Dictionary<string, object> { { "IsActive", 0 },
                { "ModifiedBy",UserID},{ "ModifiedDate",ModifiedDate} },
                    new Dictionary<string, object> { { "JobID", JobID } });
            }
            else
                return false;
     
        }
    }
}
