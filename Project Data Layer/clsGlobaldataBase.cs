using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BMS.Common;
using BMS.Entities;

public static class clsGlobalDatabase 
{


    private static readonly string _connectionString = ConfigReader.GetConnectionString();
  


    

    public static async Task <DataTable> GetAllAsyncByStoredProcedure(string ProcedureName, SqlParameter[] parameters = null)
    {
        using(SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand(ProcedureName, connection))
            {
                AddParameters(command, parameters);
                command.CommandType = CommandType.StoredProcedure;
                
                

                try
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        DataTable results = new DataTable();
                        if (await reader.ReadAsync())
                        {
                           
                            results.Load(reader);
                        }
                        return results;
                    }
                }catch(Exception ex)
                {
                    Logger.LogError($"Procedure: {ProcedureName}, Error: {ex.Message}", true);
                    throw new Exception("An error occurred while processing your request. Please contact the administrator.", ex);

                }
            }
        }     
    }

    public static async Task<Dictionary<string,object>> GetSingleRecord(string ProcedureName, SqlParameter[] parameters = null)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand(ProcedureName, connection))
            {
                AddParameters(command, parameters);
                command.CommandType = CommandType.StoredProcedure;

                Dictionary<string,object> RecordData = new Dictionary<string, object>();


                try
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                       if(reader.Read())
                       {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string ColumnName = reader.GetName(i);

                                object value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                RecordData[ColumnName] = value;
                            }
                           
                        }
                    }
                    return RecordData;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Procedure: {ProcedureName}, Error: {ex.Message}", true);
                    throw new Exception("An error occurred while processing your request. Please contact the administrator.", ex);

                }
            }
        }
    }



    public static  async Task<int> ExecuteScalarAsync(string procedureName, SqlParameter[] parameters = null)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddParameters(cmd, parameters);
                try
                {
                    await conn.OpenAsync();
                    object result = await cmd.ExecuteScalarAsync() ?? -1;
                    return (int)result;
                }
                catch (SqlException ex)
                {
                    Logger.LogError($"Procedure: {procedureName}, Error: {ex.Message}", true);
                    throw new Exception("An error occurred while processing your request. Please contact the administrator.", ex);

                }
                catch (Exception ex)
                {
                    Logger.LogError($"Procedure: {procedureName}, Error: {ex.Message}", true);
                    throw new Exception("An error occurred while processing your request. Please contact the administrator.", ex);

                }
            }
        }
    }
    private static void AddParameters(SqlCommand cmd, SqlParameter[] parameters = null)
    {
        if (parameters != null && parameters.Length > 0)
        {
            foreach (var param in parameters)
            {
                if (param.Value == null)
                {
                    param.Value = DBNull.Value;
                }
                   cmd.Parameters.Add(param);
            }
        }
    }










}




