using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;

public static class clsGlobalCrudOperations
{
    private static readonly HashSet<string> AllowedTables = new HashSet<string>
    {
    "Departments", "Job"
    };

    //private static readonly HashSet<string> AllowedTables = new HashSet<string>(
    //    (ConfigurationManager.AppSettings["AllowedTables"])
    //        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
    //        .Select(t => t.Trim()),
    //    StringComparer.OrdinalIgnoreCase
    //);


    private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
    private static readonly string _sourceName = "PMS";




    static public void LOGEventHandler(string message)
    {
        string source = _sourceName;
        string logName = "Application";

        if (!EventLog.SourceExists(source))
        {
            try { EventLog.CreateEventSource(source, logName); }
            catch (SecurityException)
            {

                Console.WriteLine("Permission denied to create event source.");
            }
            catch (IOException)
            {

                Console.WriteLine("IO Exception occurred while creating event source.");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }
        EventLog.WriteEntry(source, message, EventLogEntryType.Error);
    }



    static public async Task<bool> InsertAsync(string tableName, Dictionary<string, object> parameters)
    {
        ValidateTableName(tableName);
        bool Added = false;
        var (columns, values, sqlParams) = PrepareInsertParameters(parameters);

        var query = $"INSERT INTO {tableName} ({columns}) VALUES ({values});";
        try
        {
            using (var _connection = new SqlConnection(_connectionString))
            {
                await _connection.OpenAsync();
                using (var cmd = new SqlCommand(query, _connection))
                {
                    cmd.Parameters.AddRange(sqlParams);
                    Added = await cmd.ExecuteNonQueryAsync() > 0;

                }
            }
        } catch (Exception ex)
        {

            LOGEventHandler(ex.Message);


        }

        return Added;
    }

    static public async Task<bool> UpdateAsync(string tableName, Dictionary<string, object> parameters,
                                     Dictionary<string, object> whereParameters)
    {

        bool Updated = false;
        ValidateTableName(tableName);
        var (setClause, whereClause, sqlParams) = PrepareUpdateParameters(parameters, whereParameters);

        var query = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";
        try
        {
            using (var _connection = new SqlConnection(_connectionString))
            {
                await _connection.OpenAsync();
                using (var cmd = new SqlCommand(query, _connection))
                {
                    cmd.Parameters.AddRange(sqlParams);
                    Updated = await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
        } catch (Exception ex)
        {
            LOGEventHandler(ex.Message);

        }
        return Updated;
    }


    static public async Task<Dictionary<string, object>> GetSingleAsync(string tableName,
                                                               Dictionary<string, object> whereParameters,
                                                               IEnumerable<string> columns = null)
    {
        ValidateTableName(tableName);


        var (whereClause, sqlParams) = PrepareWhereParameters(whereParameters);
        var selectColumns = columns != null ? string.Join(", ", columns) : "*";



        var query = $"SELECT {selectColumns} FROM {tableName} WHERE {whereClause}";
        using (var _connection = new SqlConnection(_connectionString))
        {
            try
            {


                await _connection.OpenAsync();
                using (var cmd = new SqlCommand(query, _connection))
                {
                    cmd.Parameters.AddRange(sqlParams);
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow))
                    {
                        return await ReadSingleRowAsync(reader);
                    }
                }
            } catch (Exception ex)
            {
                LOGEventHandler(ex.Message);
                return null;

            }


        }
    }

    static public async Task<DataTable> GetAllAsync(string tableName, IEnumerable<string> columns = null,
                                             Dictionary<string, object> whereParameters = null)
    {
        ValidateTableName(tableName);
        var selectColumns = columns != null ? string.Join(", ", columns) : "*";
        var whereClause = "";
        SqlParameter[] sqlParams = Array.Empty<SqlParameter>();


        if (whereParameters != null && whereParameters.Count > 0)
        {
            var wherePrep = PrepareWhereParameters(whereParameters);
            whereClause = " WHERE " + wherePrep.whereClause;
            sqlParams = wherePrep.sqlParams;
        }

        var query = $"SELECT {selectColumns} FROM {tableName}{whereClause}";

        try
        {


            using (var _connection = new SqlConnection(_connectionString))
            {
                await _connection.OpenAsync();
                using (var cmd = new SqlCommand(query, _connection))
                {
                    cmd.Parameters.AddRange(sqlParams);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        return dataTable;
                    }
                }
            }
        } catch (Exception ex)
        {
            LOGEventHandler(ex.Message);
            return null;
        }


    }



    static private (string columns, string values, SqlParameter[] sqlParams) PrepareInsertParameters(
        Dictionary<string, object> parameters)
    {
        var columns = string.Join(", ", parameters.Keys);
        var values = string.Join(", ", parameters.Keys.Select(k => $"@{k}"));
        var sqlParams = parameters.Select(p => new SqlParameter($"@{p.Key}", p.Value)).ToArray();
        return (columns, values, sqlParams);
    }

    static private (string setClause, string whereClause, SqlParameter[] sqlParams) PrepareUpdateParameters(
        Dictionary<string, object> parameters, Dictionary<string, object> whereParameters)
    {
        var setClause = string.Join(", ", parameters.Keys.Select(k => $"{k} = @{k}"));
        var wherePrep = PrepareWhereParameters(whereParameters);

        var sqlParams = parameters.Select(p => new SqlParameter($"@{p.Key}", p.Value)).Concat(wherePrep.sqlParams).ToArray();


        return (setClause, wherePrep.whereClause, sqlParams);
    }

    static private (string whereClause, SqlParameter[] sqlParams) PrepareWhereParameters(
        Dictionary<string, object> parameters)
    {
        var whereClauses = parameters.Keys.Select(k => $"{k} = @{k}");
        var whereClause = string.Join(" AND ", whereClauses);
        var sqlParams = parameters.Select(p =>
            new SqlParameter($"@{p.Key}", p.Value)).ToArray();
        return (whereClause, sqlParams);
    }

    static private async Task<Dictionary<string, object>> ReadSingleRowAsync(SqlDataReader reader)
    {
        if (!await reader.ReadAsync()) return null;

        var result = new Dictionary<string, object>();
        for (var i = 0; i < reader.FieldCount; i++)
        {
            result[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader[i];
        }
        return result;
    }

    private static void ValidateTableName(string tableName)
    {
        if (!AllowedTables.Contains(tableName))
            throw new ArgumentException($"Invalid table: {tableName}");
    }


    public static async Task<bool> CanDeleteRecordAsync(string tableName, string columnName, int id)
    {
        // Ensure you're using a new connection for each query
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            string query = $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = @Id";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                var result = await command.ExecuteScalarAsync();

                return Convert.ToInt32(result) == 0; // True if no references exist
            }
        }
    }



}




