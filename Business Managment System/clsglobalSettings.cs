using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using BMS.Entities;
using System.Runtime.InteropServices;

namespace Projects_Managment_System
{
   public  class clsglobalSettings
    {
        private static readonly string _connectionString =  ConfigReader.GetConnectionString();
        static public void AdjustGridDesign(DataGridView dataGridView)
        {

           
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#2C3E50");
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.ColumnHeadersHeight = 45;
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 14F, FontStyle.Bold);


            dataGridView.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F8F9FA");
            dataGridView.DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#2C3E50");

           
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#E9ECEF");

            dataGridView.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#3498DB");
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        public static async Task<int> GetNextIDAsync(string tableName, string idColumnName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("GetNextID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    cmd.Parameters.AddWithValue("@ColumnName", idColumnName);

                    SqlParameter outputParam = new SqlParameter("@NextID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                    return Convert.ToInt32(outputParam.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while fetching next ID: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }


    }
}
