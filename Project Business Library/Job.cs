using DataLayer;
using Project_Data_Layer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Business_Library
{
   public class Job
    {
        public enum enMode { AddNew =1 ,Update =2 }

        private enMode _Mode = enMode.AddNew;
        public int JobID { get; set; }
        public string JobTitle { get; set; }
        public int DepartmentID { get; set; }
        public Department Department { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }


        public Job()
        {
            this.JobID = this.CreatedBy = this.ModifiedBy= DepartmentID = -1;
            CreationDate = ModifiedDate = DateTime.Now;
            IsActive = true;
            JobTitle = string.Empty;

            _Mode = enMode.AddNew;
        }


        private Job(int JobID,string JobTitle ,bool IsActive , int CreatedBy , int ModifiedBy , DateTime CreatedAt,
            DateTime ModifiedAt,int DepartmentID)
        {
            this.JobID= JobID;
            this.JobTitle= JobTitle;
            this.IsActive = IsActive;
            this.CreatedBy = CreatedBy;
            this.ModifiedBy = ModifiedBy;
            this.CreationDate = CreatedAt;
            this.ModifiedDate = ModifiedAt;
            this.DepartmentID = DepartmentID;

        
            _Mode = enMode.Update;
        }

        static public async Task<DataTable> GetAllJobs()
        {
            return await clsJobData.GetAllJobsAsync();
        }


        public static async Task<Job> CreateAsync(int jobID, string jobTitle, bool isActive, int createdBy, int modifiedBy, DateTime createdAt,
    DateTime modifiedAt, int departmentID)
        {
            var job = new Job(jobID, jobTitle, isActive, createdBy, modifiedBy, createdAt, modifiedAt, departmentID);
            job.Department = await Department.GetDepartment(departmentID);
            return job;
        }


        static public async Task<Job> GetJob(int JobId)
        {
            Dictionary<string, object> result = await clsJobData.GetJobByIDAsync(JobId);

            if (result != null)
            {

                return await CreateAsync(
                 (int)result["JobID"],
                 result["JobTitle"].ToString(),
                 (bool)result["IsActive"],
                 (int)result["CreatedBy"],
                 (int)result["ModifiedBy"],
                 (DateTime)result["CreatedDate"],
                 (DateTime)result["ModifiedDate"],
                 (int)result["DepartmentID"]
             );
            }
            else
            {
                return null;
            }

        }

        private async Task<bool> AddJob()
        {
            return (await clsJobData.AddJobASync(this.JobID, this.CreatedBy, this.JobTitle, DateTime.Now,this.DepartmentID));
        }

        private async Task<bool> UpdateJob()
        { 
            return await clsJobData.UpdateJobAsync(this.JobID, this.JobTitle, this.ModifiedBy, this.IsActive, DateTime.Now,this.DepartmentID);
        }



        public async Task<bool> DeactiveteJob()
        {

            return await clsJobData.DeleteJobAsync(this.DepartmentID, this.ModifiedBy, DateTime.Now);
        }



        public async Task<bool> Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (await AddJob())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return await UpdateJob();
                default:
                    return false;
            }
        }
    }
}
